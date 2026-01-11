/**
 * Mermaid Diagram Initialization Script
 * 
 * Handles theme-aware rendering of Mermaid diagrams with support for:
 * - Light/dark theme detection and switching
 * - Error handling for invalid diagram syntax
 * - MutationObserver for real-time theme changes
 */

(function () {
  // Theme color configuration
  const themeConfig = {
    light: {
      primaryColor: '#6200ee',
      primaryTextColor: '#000',
      primaryBorderColor: '#6200ee',
      background: 'transparent',
      mainBkg: '#ffffff',
      secondBkg: '#f5f5f5',
      tertiaryBkg: '#efefef',
      textColor: '#333333',
      lineColor: '#333333',
      linkColor: '#6200ee',
      tertiaryColor: '#f0f0f0'
    },
    dark: {
      primaryColor: '#bb86fc',
      primaryTextColor: '#ffffff',
      primaryBorderColor: '#bb86fc',
      background: 'transparent',
      mainBkg: '#1e1e1e',
      secondBkg: '#2d2d2d',
      tertiaryBkg: '#3a3a3a',
      textColor: '#e0e0e0',
      lineColor: '#5e5e5e',
      linkColor: '#bb86fc',
      tertiaryColor: '#404040'
    }
  };

  /**
   * Determine the current theme from the HTML data-theme attribute
   */
  function getCurrentTheme() {
    const htmlElement = document.documentElement;
    const theme = htmlElement.getAttribute('data-theme') || 'light';
    return theme === 'dark' ? 'dark' : 'light';
  }

  /**
   * Get theme configuration for the current theme
   */
  function getThemeConfig() {
    return themeConfig[getCurrentTheme()];
  }

  /**
   * Initialize Mermaid with current theme configuration
   */
  function initializeMermaid() {
    if (typeof mermaid === 'undefined') {
      console.warn('Mermaid library not loaded');
      return;
    }

    const config = getThemeConfig();
    
    mermaid.initialize({
      startOnLoad: true,
      theme: getCurrentTheme() === 'dark' ? 'dark' : 'default',
      securityLevel: 'loose',
      logLevel: 'debug',
      themeVariables: config,
      flowchart: {
        htmlLabels: true,
        useMaxWidth: true
      },
      sequence: {
        messageAlign: 'center'
      },
      gantt: {
        useWidth: undefined
      }
    });
  }

  /**
   * Render all diagrams on the page
   */
  function renderDiagrams() {
    // Find all mermaid diagram elements
    const diagrams = document.querySelectorAll('.mermaid');
    
    if (diagrams.length === 0) {
      return; // No diagrams to render
    }

    // Reinitialize mermaid with current theme
    initializeMermaid();

    // Clear any previously rendered content and re-render
    try {
      // Mermaid v10+ uses mermaid.contentLoaded() or mermaid.run()
      if (typeof mermaid.run === 'function') {
        // v10+ API
        mermaid.run();
      } else if (typeof mermaid.contentLoaded === 'function') {
        // Fallback for earlier versions
        mermaid.contentLoaded();
      }
    } catch (error) {
      console.error('Error rendering Mermaid diagrams:', error);
    }
  }

  /**
   * Set up observer for theme changes
   */
  function setupThemeObserver() {
    const htmlElement = document.documentElement;
    
    const observer = new MutationObserver((mutations) => {
      mutations.forEach((mutation) => {
        if (mutation.type === 'attributes' && mutation.attributeName === 'data-theme') {
          // Theme has changed, re-render diagrams
          renderDiagrams();
        }
      });
    });

    observer.observe(htmlElement, {
      attributes: true,
      attributeFilter: ['data-theme'],
      attributeOldValue: true
    });
  }

  /**
   * Main initialization function
   */
  function init() {
    // Check if there are any mermaid diagrams on the page
    const hasDiagrams = document.querySelector('.mermaid') !== null;
    
    if (!hasDiagrams) {
      return; // No diagrams, no need to initialize
    }

    // Wait for Mermaid library to be loaded
    if (typeof mermaid === 'undefined') {
      console.warn('Mermaid library is not loaded. Please ensure mermaid.min.js is included.');
      return;
    }

    // Initialize Mermaid
    initializeMermaid();

    // Render diagrams when the DOM is ready
    if (document.readyState === 'loading') {
      document.addEventListener('DOMContentLoaded', renderDiagrams);
    } else {
      renderDiagrams();
    }

    // Set up theme observer for real-time theme switching
    setupThemeObserver();
  }

  // Initialize when the script loads
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }
})();
