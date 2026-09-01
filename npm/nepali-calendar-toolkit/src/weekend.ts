// JS convention: 0 = Sunday ... 6 = Saturday
let weekendDays = new Set<number>([0, 6]);

export function setWeekendDays(...days: number[]): void {
  weekendDays = new Set(days);
}

export function getWeekendDays(): number[] {
  return [...weekendDays].sort((a, b) => a - b);
}

export function isWeekend(dayOfWeek: number): boolean {
  return weekendDays.has(dayOfWeek);
}
