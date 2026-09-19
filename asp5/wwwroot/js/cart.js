document.addEventListener('click', async event => {
    const button = event.target.closest('.add-to-cart');

    if (!button) {
        return;
    }

    button.disabled = true;

    const response = await fetch('/Catalog/AddToCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: 'id=' + encodeURIComponent(button.dataset.productId)
    });

    const data = await response.json();

    document.getElementById('cartBadge').textContent = data.cartCount;
    button.textContent = 'Добавлено ✓';
});
