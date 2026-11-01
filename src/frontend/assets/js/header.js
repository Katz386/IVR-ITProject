showAccount();

async function showAccount() {
    const account = document.querySelector('#account');

    if (!account) {
        return;
    }

    const token = localStorage.getItem('token');

    if (!token) {
        localStorage.removeItem('profileName');
        account.classList.add('account-ready');
        return;
    }

    const savedName = localStorage.getItem('profileName');

    showProfileLink(account, savedName || 'Мой профиль');

    try {
        const response = await fetch('/api/user/profile/me', {
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });

        if (response.status === 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('profileName');

            showLoginLinks(account);
            return;
        }

        if (!response.ok) {
            return;
        }

        const result = await response.json();

        if (!result.profile) {
            return;
        }

        const name = result.profile.name;

        localStorage.setItem('profileName', name);
        showProfileLink(account, name);
    } catch (error) {
        return;
    }
}

function showProfileLink(account, name) {
    const link = document.createElement('a');

    link.className = 'account-name';
    link.href = '/profile/me';
    link.textContent = name;

    account.textContent = '';
    account.append(link);
    account.classList.add('account-ready');
}

function showLoginLinks(account) {
    const signIn = document.createElement('a');
    signIn.className = 'button button-light';
    signIn.href = '/sign-in';
    signIn.textContent = 'Войти';

    const signUp = document.createElement('a');
    signUp.className = 'button button-dark';
    signUp.href = '/sign-up';
    signUp.textContent = 'Регистрация';

    account.textContent = '';
    account.append(signIn, signUp);
    account.classList.add('account-ready');
}