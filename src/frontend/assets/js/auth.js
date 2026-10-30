const form = document.querySelector('.auth-form');
const message = document.querySelector('#auth-message');
const button = form.querySelector('.auth-submit');

form.addEventListener('submit', sendForm);

async function sendForm(event) {
    event.preventDefault();

    const username = form.elements.username.value.trim();
    const password = form.elements.password.value;

    if (username === '') {
        message.textContent = 'Введите имя пользователя';
        return;
    }

    let url = '/api/auth/sign-in';

    if (form.id === 'sign-up-form') {
        url = '/api/auth/sign-up';
    }

    const user = {
        username: username,
        password: password
    };

    message.textContent = 'Подождите...';
    button.disabled = true;

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        });

        let result;

        try {
            result = await response.json();
        } catch (error) {
            message.textContent = 'Ошибка на стороне сервера. Попробуйте позже';
            return;
        }

        if (!response.ok) {
            message.textContent = result.message || 'Что-то пошло не так. Попробуйте позже';
            return;
        }

        if (typeof result.token !== 'string' || result.token.trim() === '') {
            message.textContent = 'Что-то пошло не так. Попробуйте позже';
            return;
        }

        localStorage.setItem('token', result.token);

        form.elements.password.value = '';

        window.location.href = '/';
    } catch (error) {
        message.textContent =
            'Не удалось завершить запрос';
    } finally {
        button.disabled = false;
    }
}