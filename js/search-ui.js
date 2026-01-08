import { querySearch, preload } from "/js/search.js";

const input = document.getElementById("search-input");
const resultsContainer = document.getElementById("search-results");

if (input) {
  input.addEventListener("focus", () => {
    preload();
    input.parentElement.classList.add("focused");
  });
  
  input.addEventListener("blur", () => {
     // Delay hiding to allow clicks on results
     setTimeout(() => {
         if (document.activeElement !== input) {
             input.parentElement.classList.remove("focused");
         }
     }, 200);
  });

  // Debounce helper
  function debounce(func, wait) {
    let timeout;
    return function(...args) {
      const context = this;
      clearTimeout(timeout);
      timeout = setTimeout(() => func.apply(context, args), wait);
    };
  }

  const handleInput = debounce(async (e) => {
    const q = e.target.value.trim();
    if (!q) {
      resultsContainer.innerHTML = "";
      resultsContainer.style.display = "none";
      return;
    }
    
    try {
        const results = await querySearch(q);
        renderResults(results);
        resultsContainer.style.display = "block";
    } catch (err) {
        console.error(err);
    }
  }, 300);

  input.addEventListener("input", handleInput);
}

function renderResults(results) {
  if (!results.length) {
    resultsContainer.innerHTML = "<div class='search-no-results'>No results found.</div>";
    return;
  }
  
  const html = results.map(doc => `
    <div class="search-result">
      <a href="${doc.url}">
        <h4>${doc.title}</h4>
        <p>${doc.excerpt || ""}</p>
      </a>
    </div>
  `).join("");
  
  resultsContainer.innerHTML = html;
}
