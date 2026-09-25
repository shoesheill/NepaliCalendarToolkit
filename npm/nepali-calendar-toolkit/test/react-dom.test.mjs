// DOM smoke: mounts the React popup in jsdom and checks the header dropdowns —
// month BEFORE year, 12 localised month options, and that picking a month in the
// select actually swaps the rendered grid.
import assert from "node:assert";
import { JSDOM } from "jsdom";

const dom = new JSDOM('<!doctype html><html><body><div id="root"></div><div id="root2"></div><div id="root3"></div><div id="root4"></div></body></html>', {
  pretendToBeVisual: true,
  url: "http://localhost/",
});
globalThis.window = dom.window;
globalThis.document = dom.window.document;
globalThis.HTMLElement = dom.window.HTMLElement;
globalThis.Node = dom.window.Node;
// Node 24 exposes a read-only global `navigator`, so it can only be redefined.
Object.defineProperty(globalThis, "navigator", {
  value: dom.window.navigator,
  configurable: true,
  writable: true,
});

// Globals must exist before React (and its DOM renderer) are evaluated.
const React = (await import("react")).default;
const { createRoot } = await import("react-dom/client");
const { BsDatePickerPopup } = await import("../dist/react/index.js");
const { ready } = await import("../dist/index.js");
await ready;

const waitUntil = async (cond, label) => {
  const deadline = Date.now() + 15000;
  while (!cond() && Date.now() < deadline) await new Promise((r) => setTimeout(r, 25));
  assert.ok(cond(), `timeout waiting for ${label}`);
};

const root = createRoot(document.getElementById("root"));
root.render(React.createElement(BsDatePickerPopup, {
  open: true,
  value: "2083-05-15",
  locale: "en",
  onOpenChange: () => {},
}));

await waitUntil(() => document.querySelector(".nct-month"), "the month select");

// ── Header order: month select, then year select ──────────────────────────────
const selects = Array.from(document.querySelectorAll(".nct-title select"));
assert.strictEqual(selects.length, 2, `expected 2 header selects, got ${selects.length}`);
const [monthSel, yearSel] = selects;
assert.ok(monthSel.classList.contains("nct-month"), "first header select must be the month");
assert.ok(yearSel.classList.contains("nct-year"), "second header select must be the year");
assert.strictEqual(monthSel.getAttribute("aria-label"), "Month");
assert.strictEqual(yearSel.getAttribute("aria-label"), "Year");

// ── Options: 12 BS months, localised from the toolkit ─────────────────────────
assert.strictEqual(monthSel.options.length, 12);
assert.strictEqual(monthSel.options[0].textContent, "Baisakh");
assert.strictEqual(monthSel.options[3].textContent, "Shrawan");
assert.strictEqual(monthSel.value, "5", "month select should reflect the selected BS month");
assert.strictEqual(yearSel.value, "2083");

// ── Picking a month in the dropdown swaps the grid ────────────────────────────
await waitUntil(() => document.querySelectorAll(".nct-day").length > 0, "the day grid");
const { convertToAd } = await import("../dist/index.js");
const adOf = (year, month, day) => convertToAd({ year, month, day });
// Each day cell carries its resolved AD date in `title`, so day 1 of the view
// month is a reliable marker for WHICH month is rendered. It is null while the
// next grid loads (placeholder days render instead).
const firstDayTitle = () => document.querySelector(".nct-day")?.getAttribute("title") ?? null;
assert.strictEqual(firstDayTitle(), await adOf(2083, 5, 1), "should start on Bhadra 1, 2083");

const setNativeValue = Object.getOwnPropertyDescriptor(dom.window.HTMLSelectElement.prototype, "value").set;
setNativeValue.call(monthSel, "6");
monthSel.dispatchEvent(new dom.window.Event("change", { bubbles: true }));

await waitUntil(() => document.querySelector(".nct-month").value === "6", "month 6 to be selected");
const ashwinFirst = await adOf(2083, 6, 1);
await waitUntil(() => firstDayTitle() === ashwinFirst, "the Ashwin 2083 grid");
assert.strictEqual(yearSel.value, "2083", "year must not change when picking a month");
assert.strictEqual(document.querySelectorAll(".nct-day").length, 31, "Ashwin 2083 has 31 days");

// ── Holiday marking: red digit only (no dot), named for screen readers ────────
// Return to Bhadra before inspecting the rich event cell.
const monthSel2 = document.querySelector(".nct-month");
setNativeValue.call(monthSel2, "5");
monthSel2.dispatchEvent(new dom.window.Event("change", { bubbles: true }));
const month5FirstAd = await adOf(2083, 5, 1);
await waitUntil(() => firstDayTitle() === month5FirstAd, "the Bhadra 2083 (month 5) grid");

// Bhadra 2083 (month 5): day 12 is Rakshya Bandhan on a Friday. Verified against
// the rich Events/2083.json data.
await waitUntil(() => document.querySelectorAll(".nct-day").length === 31, "all 31 days of month 5");
const days = Array.from(document.querySelectorAll(".nct-day"));
assert.strictEqual(days[11].querySelector(".nct-dot"), null, "no dot marker anywhere");
assert.strictEqual(document.querySelectorAll(".nct-dot").length, 0, "the grid must not render any dot");
assert.ok(days[11].classList.contains("nct-hol"), "event day should use the event class");
assert.ok(days[11].classList.contains("nct-we"), "an event keeps the off-day class too");
assert.ok(!days[0].classList.contains("nct-hol"), "a plain day must not use the holiday class");

// The holiday is carried by the accessible name + tooltip instead of a glyph.
assert.strictEqual(days[11].getAttribute("aria-label"), "12, Fri, Rakshya Bandhan");
assert.strictEqual(days[0].getAttribute("aria-label"), "1, Mon");
assert.strictEqual(days[11].getAttribute("title"), "Rakshya Bandhan");

// The legend is a single red line (no dot swatch), localised from `locale`.
const legend = document.querySelector(".nct-legend");
assert.ok(legend, "expected a legend row under the grid");
assert.strictEqual(legend.textContent, "Public holiday / weekly off");
assert.strictEqual(legend.querySelectorAll(".nct-dot").length, 0, "legend must not show a dot");

root.unmount();

// ── A SELECTED holiday keeps the red/holiday info without any dot ────────────
const root2 = createRoot(document.getElementById("root2"));
root2.render(React.createElement(BsDatePickerPopup, {
  open: true,
  value: "2083-05-12",
  locale: "en",
  onOpenChange: () => {},
}));
await waitUntil(() => document.querySelector("#root2 .nct-day"), "the second popup grid");
const selDays = Array.from(document.querySelectorAll("#root2 .nct-day"));
await waitUntil(() => selDays[11].classList.contains("nct-sel"), "day 12 to render as selected");
assert.strictEqual(selDays[11].querySelector(".nct-dot"), null, "selected day renders no dot");
// A selected cell is filled with the accent colour, so the red cue gives way to
// the selection styling — the holiday stays available via title + aria-label.
assert.ok(selDays[11].classList.contains("nct-sel"), "selected day keeps the selection class");
assert.ok(!selDays[11].classList.contains("nct-hol"), "selection replaces the red event cue");
assert.strictEqual(selDays[11].getAttribute("title"), "Rakshya Bandhan", "tooltip survives selection");
assert.strictEqual(selDays[11].getAttribute("aria-label"), "12, Fri, Rakshya Bandhan", "name survives selection");
root2.unmount();

// ── NEPALI LOCALE: digits are Devanagari, headings and legend localised ───────
const root3 = createRoot(document.getElementById("root3"));
root3.render(React.createElement(BsDatePickerPopup, {
  open: true,
  value: "2083-05-12",
  locale: "ne",
  onOpenChange: () => {},
}));
await waitUntil(() => document.querySelector("#root3 .nct-day"), "the Nepali popup grid");
const neDays = Array.from(document.querySelectorAll("#root3 .nct-day"));
await waitUntil(() => neDays.length === 31, "all 31 Nepali-language days");

// Day numbers: Devanagari digits, concatenated with the '-' in cell aria labels.
assert.strictEqual(neDays[0].textContent, "१", "day 1 must render as १");
assert.strictEqual(neDays[11].textContent, "१२", "day 12 must render as १२");
assert.ok(/^[०-९]+$/.test(neDays[10].textContent), `expected Devanagari digits, got ${neDays[10].textContent}`);

// Weekday headings + month names come localised from the toolkit (ne).
const neSelects = Array.from(document.querySelectorAll("#root3 .nct-title select"));
assert.strictEqual(neSelects[0].options[3].textContent, "श्रावण", "month names must be Devanagari");
assert.strictEqual(neSelects[0].value, "5", "option VALUES stay Latin even when labels are Devanagari");
assert.ok(/^[०-९]+$/.test(neSelects[1].options[0].textContent), "year labels use Devanagari digits");
assert.ok(/^[०-९]{4}$/.test(neSelects[1].options[0].textContent), "year labels are 4 Devanagari digits, no Latin");
const neHeaders = Array.from(document.querySelectorAll("#root3 .nct-wd")).map((n) => n.textContent);
assert.deepStrictEqual(neHeaders, ["आइत", "सोम", "मंगल", "बुध", "बिही", "शुक्र", "शनि"]);

// Accessible name is Devanagari too, but keeps the English holiday name from data.
assert.strictEqual(neDays[11].getAttribute("aria-label"), "१२, शुक्र, Rakshya Bandhan");
// The AD date is a Gregorian value, so it deliberately stays Latin.
assert.strictEqual(neDays[11].getAttribute("title"), "Rakshya Bandhan");
const neAdLine = document.querySelector("#root3 .nct-ad").textContent;
assert.ok(neAdLine.includes("2026-08-28"), `AD stays Latin: ${neAdLine}`);
assert.ok(neAdLine.includes("२०८३-०५-१२"), `BS is Devanagari: ${neAdLine}`);
assert.strictEqual(document.querySelector("#root3 .nct-legend").textContent, "सार्वजनिक बिदा / साप्ताहिक बिदा");
root3.unmount();

// ── Explicit override wins: locale="ne" + nepaliDigits={false} → Latin ────────
const root4 = createRoot(document.getElementById("root4"));
root4.render(React.createElement(BsDatePickerPopup, {
  open: true,
  value: "2083-05-12",
  locale: "ne",
  nepaliDigits: false,
  onOpenChange: () => {},
}));
await waitUntil(() => document.querySelector("#root4 .nct-day"), "the override popup grid");
const latDays = Array.from(document.querySelectorAll("#root4 .nct-day"));
await waitUntil(() => latDays.length === 31, "all 31 override days");
assert.strictEqual(latDays[11].textContent, "12", "nepaliDigits={false} must force Latin digits");
assert.strictEqual(latDays[0].textContent, "1", "nepaliDigits={false} must force Latin digits");
assert.strictEqual(
  Array.from(document.querySelectorAll("#root4 .nct-title select"))[0].options[3].textContent,
  "श्रावण",
  "names stay localised; only the digits flip back to Latin",
);
root4.unmount();

console.log("\nnepali-calendar-toolkit /react dom smoke passed");