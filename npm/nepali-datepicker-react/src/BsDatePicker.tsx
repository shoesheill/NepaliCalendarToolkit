// --- BsDatePicker (React) ---
// Thin render layer over the headless controller: popup month grid with
// holidays, weekends, min/max bounds. Zero styling-framework lock-in: plain
// `nct-` CSS classes (import './styles.css') + opt-in `classNames` overrides.
// The host keeps its own trigger (button/input) and renders ONLY the popup,
// or uses the bundled `BsDateTrigger` below.

import { useEffect, useMemo, useRef, useState } from "react";
import {
  createBsPickerController,
  isBsKeyDisabled,
  type BsPickerViewState,
} from "./controller";
import { sameBsDay } from "nepali-calendar-toolkit";
import {
  formatBsLabel,
  normalizeBsKey,
  toPickerDigits,
  type BsDatePickerOptions,
  type BsTriggerOptions,
} from "./props";
import type { BsDateString } from "./props";

function cx(...parts: Array<string | undefined | false>): string {
  return parts.filter(Boolean).join(" ");
}

function usePickerState(opts: BsDatePickerOptions, open: boolean) {
  const [state, setState] = useState<BsPickerViewState>({
    viewYear: null, viewMonth: null, grid: null, loading: false,
    years: [], months: [], monthNames: [], disabledMonths: [],
    monthLabel: "\u2014", weekdayLabels: [], todayKey: null,
  });
  const optsRef = useRef(opts);
  optsRef.current = opts;
  const ctrlRef = useRef<ReturnType<typeof createBsPickerController> | null>(null);
  if (!ctrlRef.current) {
    ctrlRef.current = createBsPickerController(
      {
        value: opts.value, initialMonth: opts.initialMonth, min: opts.min,
        max: opts.max, disabledDates: opts.disabledDates,
        disabledWeekdays: opts.disabledWeekdays, locale: opts.locale,
        onChange: (s) => optsRef.current.onChange?.(s),
      },
      setState,
    );
  }
  useEffect(() => {
    ctrlRef.current?.setOptions({
      value: opts.value, initialMonth: opts.initialMonth, min: opts.min,
      max: opts.max, disabledDates: opts.disabledDates,
      disabledWeekdays: opts.disabledWeekdays, locale: opts.locale,
      onChange: (s) => optsRef.current.onChange?.(s),
    });
  }, [opts.value, opts.initialMonth, opts.min, opts.max, opts.disabledDates, opts.disabledWeekdays, opts.locale]);
  useEffect(() => { if (open) ctrlRef.current?.open(); }, [open]);
  useEffect(() => () => ctrlRef.current?.dispose(), []);
  return { state, ctrl: ctrlRef.current };
}

export interface BsDatePickerPopupProps extends BsDatePickerOptions {
  open: boolean;
  onOpenChange?: (open: boolean) => void;
}

export function BsDatePickerPopup(props: BsDatePickerPopupProps) {
  const { open, onOpenChange, showAdLine = true, className, classNames, disabled } = props;
  const { state, ctrl } = usePickerState(props, open);
  const wrapRef = useRef<HTMLDivElement>(null);
  const locale = props.locale ?? "en";
  // Devanagari digits are the default under a Nepali locale; an explicit
  // `nepaliDigits={false}` still wins for hosts that want Latin numerals.
  const nepaliDigits = props.nepaliDigits ?? locale === "ne";
  const selectedKey = normalizeBsKey(props.value ?? null);

  useEffect(() => {
    if (!open) return;
    const onPointerDown = (e: PointerEvent) => {
      if (wrapRef.current && !wrapRef.current.contains(e.target as Node)) onOpenChange?.(false);
    };
    const onKeyDown = (e: KeyboardEvent) => { if (e.key === "Escape") onOpenChange?.(false); };
    document.addEventListener("pointerdown", onPointerDown);
    document.addEventListener("keydown", onKeyDown);
    return () => {
      document.removeEventListener("pointerdown", onPointerDown);
      document.removeEventListener("keydown", onKeyDown);
    };
  }, [open, onOpenChange]);

  const adOfSelected = useAdOf(selectedKey);

  const weekdayOf = useMemo(() => {
    const g = state.grid;
    return (k: BsDateString): number | undefined =>
      g?.cells.find((c) => sameBsDay(c.bsKey, k))?.weekday;
  }, [state.grid]);

  if (!open) return null;
  const cnm = classNames ?? {};
  const legend = locale === "ne"
    ? { holiday: "सार्वजनिक बिदा", weekend: "साप्ताहिक बिदा" }
    : { holiday: "Public holiday", weekend: "weekly off" };
  return (
    <div ref={wrapRef} className={cx("nct-popup", cnm.root, className)} data-nct="popup">
      <div className={cx("nct-header", cnm.header)}>
        <button type="button" aria-label="Previous month" disabled={disabled} onClick={() => ctrl.stepMonth(-1)} className={cx("nct-nav", cnm.navButton)}>‹</button>
        <div className={cx("nct-title", cnm.title)}>
          {state.months.length > 0 && state.viewMonth != null ? (
            <select
              aria-label="Month"
              value={state.viewMonth}
              disabled={disabled}
              onChange={(e) => ctrl.setMonth(Number(e.target.value))}
              className={cx("nct-month", cnm.monthSelect)}
            >
              {state.months.map((m) => (
                <option key={m} value={m} disabled={state.disabledMonths.includes(m)}>
                  {state.monthNames[m - 1] ?? m}
                </option>
              ))}
            </select>
          ) : (
            <span>{state.monthLabel}</span>
          )}
          <select
            aria-label="Year"
            value={state.viewYear ?? ""}
            disabled={disabled}
            onChange={(e) => ctrl.setYear(Number(e.target.value))}
            className={cx("nct-year", cnm.yearSelect)}
          >
            {(state.years.length ? state.years : state.viewYear != null ? [state.viewYear] : []).map((y) => (
              <option key={y} value={y}>{toPickerDigits(y, nepaliDigits)}</option>
            ))}
          </select>
        </div>
        <button type="button" aria-label="Next month" disabled={disabled} onClick={() => ctrl.stepMonth(1)} className={cx("nct-nav", cnm.navButton)}>›</button>
      </div>
      <div className={cx("nct-weekdays", cnm.weekdayRow)}>
        {state.weekdayLabels.map((h) => (<span key={h} className={cx("nct-wd", cnm.weekday)}>{h}</span>))}
      </div>
      {state.loading && !state.grid ? (
        <div className={cx("nct-grid", cnm.grid)}>
          {Array.from({ length: 31 }, (_, i) => (
            <span key={i} className="nct-ph">{toPickerDigits(i + 1, nepaliDigits)}</span>
          ))}
        </div>
      ) : state.grid ? (
        <div className={cx("nct-grid", cnm.grid)}>
          {Array.from({ length: state.grid.leadingBlanks }, (_, i) => (<span key={`b-${i}`} />))}
          {state.grid.cells.map((cell) => {
            const key = normalizeBsKey(cell.bsKey);
            const off = isBsKeyDisabled(key, props, weekdayOf);
            const selected = key != null && selectedKey != null && sameBsDay(key, selectedKey);
            // Weekend/holiday days are marked by red text alone, so the holiday name
            // has to ride along on the button's accessible name (and the `title`
            // tooltip) instead of a marker glyph. Weekday label is already localised.
            const dayLabel = [
              toPickerDigits(cell.day, nepaliDigits),
              state.weekdayLabels[cell.weekday],
              cell.holidayName,
            ].filter(Boolean).join(", ");
            return (
              <button
                key={cell.bsKey}
                type="button"
                title={cell.holidayName ?? cell.adDate}
                aria-label={dayLabel}
                disabled={disabled || off}
                onClick={() => { void ctrl.pickDay(cell.day).then(() => onOpenChange?.(false)); }}
                className={cx(
                  "nct-day", cnm.day,
                  selected && cx("nct-sel", cnm.daySelected),
                  !selected && cell.today && cx("nct-today", cnm.dayToday),
                  !selected && (cell.weekend || cell.holidayName) && cx("nct-we", cnm.dayWeekend),
                  !selected && cell.holidayName && cx("nct-hol", cnm.dayHoliday),
                  off && cx("nct-off", cnm.dayDisabled),
                )}
              >
                {toPickerDigits(cell.day, nepaliDigits)}
              </button>
            );
          })}
        </div>
      ) : (
        <p className={cx("nct-empty", cnm.empty)}>This month is not published yet.</p>
      )}
      {state.grid && (
        <div className={cx("nct-legend", cnm.legend)}>
          {/* Red digits are the only "off day" cue, so no dot marker is rendered. */}
          <span className={cx("nct-lg", "nct-num", cnm.legendItem)}>
            {`${legend.holiday} / ${legend.weekend}`}
          </span>
        </div>
      )}
      {showAdLine && (
        <p className={cx("nct-ad", cnm.adLine)}>AD {adOfSelected ?? "\u2014"} · {locale === "ne" ? "\u0935\u093f.\u0938\u0902." : "B.S."} {selectedKey ? toPickerDigits(selectedKey, nepaliDigits) : "\u2014"}</p>
      )}
    </div>
  );
}

function useAdOf(selectedKey: string | null): string | null {
  const [ad, setAd] = useState<string | null>(null);
  useEffect(() => {
    if (!selectedKey) { setAd(null); return; }
    let alive = true;
    void import("nepali-calendar-toolkit").then((nct) => {
      const p = selectedKey.split("-").map(Number);
      return nct.convertToAd({ year: p[0], month: p[1], day: p[2] } as never).then((a) => {
        if (alive) setAd(a);
      }).catch(() => { if (alive) setAd(null); });
    }).catch(() => { if (alive) setAd(null); });
    return () => { alive = false; };
  }, [selectedKey]);
  return ad;
}

export function BsDateTrigger(opts: BsTriggerOptions & { open?: boolean; onToggle?: () => void }) {
  const locale = opts.locale ?? "en";
  const label = opts.value
    ? formatBsLabel(opts.value, locale, opts.nepaliDigits ?? locale === "ne")
    : (opts.placeholder ?? "Select date");
  return (
    <button
      type="button"
      disabled={opts.disabled || !opts.value}
      onClick={opts.onToggle}
      aria-expanded={opts.open}
      className={cx("nct-trigger", opts.classNames?.root, opts.className)}
      data-nct="trigger"
    >
      <span aria-hidden className={cx("nct-cal", opts.classNames?.icon)}>☷</span>
      <span className={cx("nct-label", opts.classNames?.label)}>{label}</span>
    </button>
  );
}

export interface BsDatePickerProps extends BsDatePickerOptions {
  triggerClassName?: string;
}

export function BsDatePicker(props: BsDatePickerProps) {
  const [open, setOpen] = useState(props.open ?? false);
  useEffect(() => { if (props.open !== undefined) setOpen(props.open); }, [props.open]);
  const setOpenBoth = (o: boolean) => { setOpen(o); props.onOpenChange?.(o); };
  return (
    <div className="nct-wrap" data-nct="wrap">
      <BsDateTrigger
        value={props.value} locale={props.locale} nepaliDigits={props.nepaliDigits}
        placeholder={props.placeholder} disabled={props.disabled}
        className={props.triggerClassName} open={open} onToggle={() => setOpenBoth(!open)}
      />
      <BsDatePickerPopup {...props} open={open} onOpenChange={setOpenBoth} />
    </div>
  );
}
