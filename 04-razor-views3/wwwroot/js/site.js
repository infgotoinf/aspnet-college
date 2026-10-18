// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// document.querySelectorAll(".alert .btn-close").forEach(button => {
//     button.addEventListener("click", () => {
//         button.closest(".alert").remove();
//     });
// });

fetch('/Catalog/GetCartCount')
    .then(response => response.json())
    .then(data => {
        const badge = document.getElementById('cartBadge');

        if (badge) {
            badge.textContent = data.count;
        }
    });
