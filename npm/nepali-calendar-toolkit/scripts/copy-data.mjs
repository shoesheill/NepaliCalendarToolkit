import { cpSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const root = join(dirname(fileURLToPath(import.meta.url)), "..");
cpSync(join(root, "src", "data"), join(root, "dist", "data"), { recursive: true });
console.log("copied baseline data to dist/data");
