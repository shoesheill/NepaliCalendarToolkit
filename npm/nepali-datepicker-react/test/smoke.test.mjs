// Smoke: controller opens on a known month, builds a grid, switches months via
// the header dropdown, and commits a pick.
import assert from "node:assert";
import { createBsPickerController, normalizeBsKey } from "../dist/index.js";
import { ready } from "nepali-calendar-toolkit";

await ready;
assert.strictEqual(normalizeBsKey("2082-4-5"), "2082-04-05");

const waitUntil = async (cond, label) => {
  const deadline = Date.now() + 15000;
  while (!cond() && Date.now() < deadline) await new Promise((r) => setTimeout(r, 50));
  assert.ok(cond(), `timeout waiting for ${label}`);
};

let latest = null;
const ctrl = createBsPickerController(
  { value: "2082-04-15", locale: "en", onChange: () => {} },
  (s) => { latest = s; },
);
ctrl.open();
await waitUntil(() => latest?.grid, "the 2082-04 grid");
assert.strictEqual(latest.grid.year, 2082);
assert.strictEqual(latest.grid.month, 4);
assert.ok(latest.grid.cells.length >= 29 && latest.grid.cells.length <= 32);

// Header dropdowns: 12 months + 12 localised names, index-aligned with 1–12.
assert.deepStrictEqual(latest.months, [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]);
assert.strictEqual(latest.monthNames.length, 12);
assert.strictEqual(latest.monthNames[3], "Shrawan");
assert.ok(Array.isArray(latest.disabledMonths), "disabledMonths missing");
assert.ok(latest.years.length > 0, "expected a year window");

// Month dropdown switches the view and reloads the grid.
ctrl.setMonth(5);
await waitUntil(() => latest?.viewMonth === 5 && latest?.grid?.month === 5, "the 2082-05 grid");
assert.strictEqual(latest.grid.year, 2082);

// Out-of-range months are refused, so the view stays put.
ctrl.setMonth(99);
assert.strictEqual(latest.viewMonth, 5);

// Localised month names come from the toolkit (ne → Devanagari).
ctrl.setOptions({ value: "2082-04-15", locale: "ne", onChange: () => {} });
assert.strictEqual(latest.monthNames[3], "श्रावण");

let picked = null;
ctrl.setOptions({ value: "2082-04-15", locale: "en", onChange: (s) => { picked = s; } });
ctrl.setMonth(4);
await waitUntil(() => latest?.grid?.month === 4, "the 2082-04 grid again");
await ctrl.pickDay(16);
assert.ok(picked, "expected onChange after pickDay");
assert.strictEqual(picked.bs, "2082-04-16");
assert.ok(picked.ad && /^\d{4}-\d{2}-\d{2}$/.test(picked.ad), `bad AD: ${picked.ad}`);
ctrl.dispose();
console.log("\nnepali-datepicker-react smoke passed");
