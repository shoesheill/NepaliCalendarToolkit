// Ships the plain-CSS defaults alongside the compiled JS so hosts can
// `import "nepali-calendar-toolkit/styles.css"`. `dist` is the only folder in
// the published tarball, so the CSS has to land there.
import { copyFileSync, mkdirSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const root = join(dirname(fileURLToPath(import.meta.url)), "..");
mkdirSync(join(root, "dist"), { recursive: true });
copyFileSync(join(root, "src", "react", "styles.css"), join(root, "dist", "styles.css"));
console.log("copied src/react/styles.css -> dist/styles.css");
