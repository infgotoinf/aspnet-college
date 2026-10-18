const input = document.getElementById('searchInput');
const grid = document.getElementById('catalogGrid');
let timer;

input?.addEventListener('input', () => {
    clearTimeout(timer);

    timer = setTimeout(async () => {
        const query = input.value.trim();

        const response = await fetch(
            '/Catalog/Search?query=' + encodeURIComponent(query)
        );

        grid.innerHTML = await response.text();
    }, 300);
});
