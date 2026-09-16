import { execFileSync } from "node:child_process";
import { mkdtempSync, readFileSync, rmSync } from "node:fs";
import { tmpdir } from "node:os";
import { join } from "node:path";
import { createContext, runInContext } from "node:vm";
import assert from "node:assert";

/**
 * Guards the browser build. The Node smoke test cannot catch this class of bug: the
 * library used to read `process.env.DATA_URL` at module scope and load its baseline with
 * `require(`./data/${path}`)`. Both are fine under Node and both are fatal in a browser —
 * importing the module threw ReferenceError before any code could run, so every
 * conversion silently failed for every web consumer.
 *
 * Bundles for the browser, then evaluates with NO `process` and NO `require` in scope.
 */
const out = mkdtempSync(join(tmpdir(), "nct-browser-"));
const bundle = join(out, "bundle.js");

try {
  execFileSync(
    "npx",
    ["--yes", "esbuild", "src/index.ts", "--bundle", "--platform=browser",
     "--format=iife", "--global-name=NCT", `--outfile=${bundle}`, "--log-level=error"],
    { stdio: "inherit", shell: process.platform === "win32" },
  );

  const code = readFileSync(bundle, "utf8");

  // A bundler leaving either of these in browser output means the source reached for a
  // Node-only global.
  assert.ok(!/\bprocess\.env\.[A-Z_]+/.test(code.replace(/typeof process[^;]*/g, "")),
    "browser bundle still reads process.env directly");
  assert.ok(!/\brequire\(`/.test(code), "browser bundle still contains a dynamic require");

  // Evaluate with a browser-shaped global: no process, no require, no module.
  const sandbox = { console, URL, TextDecoder, AbortSignal, fetch: async () => { throw new Error("offline"); } };
  createContext(sandbox);
  runInContext(code, sandbox);

  assert.ok(sandbox.NCT, "bundle did not evaluate in a browser-shaped context");

  // Offline: fetch throws, so this exercises the BUNDLED BASELINE path — the one the old
  // dynamic require broke.
  const years = sandbox.NCT.getAvailableCalendarYearsBs?.() ?? { minYear: 0, maxYear: 0 };
  await sandbox.NCT.ready;
  const after = sandbox.NCT.getAvailableCalendarYearsBs();
  assert.ok(after.maxYear > 2070, `baseline did not load offline in browser (got ${JSON.stringify(after)})`);

  const bs = await sandbox.NCT.convertToNepali(new Date("2024-04-13T00:00:00+05:45"));
  assert.strictEqual(`${bs.year}-${bs.month}-${bs.day}`, "2081-1-1",
    "browser conversion disagrees with the known Nepali New Year");

  console.log(`browser bundle OK — baseline years ${after.minYear}-${after.maxYear}, 2024-04-13 -> ${bs.toString()}`);
  console.log("Browser tests passed ✔");
} finally {
  rmSync(out, { recursive: true, force: true });
}
