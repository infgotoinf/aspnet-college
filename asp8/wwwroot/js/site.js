(() => {
    document.addEventListener("submit", async (event) => {
        const form = event.target.closest("[data-complete-form]");

        if (!form) {
            return;
        }

        event.preventDefault();

        const button = form.querySelector('button[type="submit"]');
        const card = form.closest("[data-task-card]");
        const message = card.querySelector("[data-task-message]");
        const csrfToken = document.querySelector('meta[name="csrftoken"]')?.content;

        if (!csrfToken) {
            message.textContent = "Не удалось получить CSRF-токен.";
            return;
        }

        button.disabled = true;
        message.textContent = "";

        try {
            const response = await fetch(form.action, {
                method: "POST",
                headers: {
                    "X-CSRF-TOKEN": csrfToken,
                    "Accept": "application/json"
                }
            });

            if (!response.ok) {
                message.textContent = `Не удалось обновить задачу (HTTP ${response.status}).`;
                button.disabled = false;
                return;
            }

            const badge = document.createElement("span");
            badge.className = "badge bg-success";
            badge.textContent = "Выполнена";
            form.replaceWith(badge);
        } catch {
            message.textContent = "Ошибка соединения. Попробуйте ещё раз.";
            button.disabled = false;
        }
    });
})();
