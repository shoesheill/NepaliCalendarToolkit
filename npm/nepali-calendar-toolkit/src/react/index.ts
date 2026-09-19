/**
 * React entry point — `import { ... } from "nepali-calendar-toolkit/react"`.
 *
 * Lives at a subpath ON PURPOSE: importing React's JSX runtime would otherwise
 * make `react` a hard dependency of the whole package, and the core entry point
 * is used by server-side scripts that have no business installing React. The
 * subpath keeps `import "nepali-calendar-toolkit"` React-free (react is an
 * optional peer), while still shipping everything from ONE tarball and ONE
 * publish step — the toolkit, the headless grid, and the React components.
 */
export * from "./props";
export * from "./controller";
export { BsDatePicker, BsDatePickerPopup, BsDateTrigger, type BsDatePickerProps, type BsDatePickerPopupProps } from "./BsDatePicker";
