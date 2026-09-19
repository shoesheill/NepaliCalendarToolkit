// Boundary guard for the single-package layout: the /react subpath must not leak
// React into the CORE entry point. If it did, `npm install nepali-calendar-toolkit`
// would force every server-side consumer to install React too — which is exactly
// why the React components live behind `nepali-calendar-toolkit/react`.
//
// Static scan instead of a runtime check: react IS installed in this repo as a dev
// dependency, so `require("../dist/index.js")` would happily succeed either way.
import assert from "node:assert";
import { readFileSync, readdirSync, statSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const dist = join(dirname(fileURLToPath(import.meta.url)), "..", "dist");
const REACT_REQUIRE = /require\(\s*["']react(\/[^"']*)?["']\s*\)/;

/** Every compiled .js under `dir`, recursively. */
const walk = (dir) =>
  readdirSync(dir).flatMap((entry) => {
    const full = join(dir, entry);
    return statSync(full).isDirectory() ? walk(full) : full.endsWith(".js") ? [full] : [];
  });

const requiresReact = (file) => REACT_REQUIRE.test(readFileSync(file, "utf8"));

// Core: dist/index.js and everything it can reach — i.e. dist minus dist/react.
const coreFiles = walk(dist).filter((f) => !f.startsWith(join(dist, "react")));
assert.ok(coreFiles.length > 5, `expected a built core, found ${coreFiles.length} files`);
const polluted = coreFiles.filter(requiresReact);
assert.deepStrictEqual(
  polluted,
  [],
  `core entry point must not require react — offending files:\n${polluted.join("\n")}`,
);

// Positive control, so a typo in the pattern can't make this test pass vacuously:
// the /react build MUST pull React in.
const reactFiles = walk(join(dist, "react"));
assert.ok(reactFiles.length > 0, "expected a built /react subpath in dist");
assert.ok(
  reactFiles.some(requiresReact),
  "/react build should require react — the guard above would be meaningless otherwise",
);

console.log(
  `core stays React-free ✔ (${coreFiles.length} core files scanned, ${reactFiles.length} /react files)`,
);
