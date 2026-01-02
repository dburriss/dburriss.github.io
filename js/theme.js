(() => {
  const storageKey = "theme";

  function systemTheme() {
    try {
      return window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches
        ? "dark"
        : "light";
    } catch {
      return "light";
    }
  }

  function storedTheme() {
    try {
      const value = localStorage.getItem(storageKey);
      return value === "dark" || value === "light" ? value : null;
    } catch {
      return null;
    }
  }

  function currentTheme() {
    return document.documentElement.dataset.theme || systemTheme();
  }

  function applyTheme(theme) {
    document.documentElement.dataset.theme = theme;
  }

  function updateToggle(toggle) {
    const theme = currentTheme();
    const label = theme === "dark" ? "Dark" : "Light";
    toggle.textContent = label;
    toggle.setAttribute("aria-label", `Toggle theme (currently ${label})`);
  }

  function toggleTheme() {
    const next = currentTheme() === "dark" ? "light" : "dark";
    applyTheme(next);

    try {
      localStorage.setItem(storageKey, next);
    } catch {
      // ignore
    }

    const toggle = document.getElementById("theme-toggle");
    if (toggle) updateToggle(toggle);
  }

  // Apply stored preference (if any). Otherwise CSS handles prefers-color-scheme.
  const initial = storedTheme();
  if (initial) applyTheme(initial);

  document.addEventListener("DOMContentLoaded", () => {
    const toggle = document.getElementById("theme-toggle");
    if (!toggle) return;

    updateToggle(toggle);
    toggle.addEventListener("click", toggleTheme);
  });
})();
