document.querySelector('#logout').addEventListener('click', () => {
    localStorage.removeItem('token');
    localStorage.removeItem('profileName');
    window.location.href = '/';
});

loadProfile();

async function loadProfile() {
    const message = document.querySelector('#profile-message');
    const login = document.querySelector('#profile-login');
    const token = localStorage.getItem('token');
    const parts = window.location.pathname.split('/');
    let id = parts[2];

    if (!id) {
        message.textContent = 'Не указан пользователь';
        return;
    }

    id = id.toLowerCase();

    if (id === 'me' && !token) {
        message.textContent = 'Войдите, чтобы открыть свой профиль';
        login.hidden = false;
        return;
    }

    const headers = {};

    if (token) {
        headers.Authorization = 'Bearer ' + token;
    }

    try {
        const response = await fetch('/api/user/profile/' + encodeURIComponent(id), {
            headers: headers
        });

        if (response.status === 401) {
            localStorage.removeItem('token');
            message.textContent = 'Сессия закончилась. Войдите заново';
            login.hidden = false;
            return;
        }

        if (response.status === 403) {
            message.textContent = 'Приватный профиль';
            return;
        }

        if (response.status === 404) {
            message.textContent = 'Профиль не найден';
            return;
        }

        if (response.status === 400) {
            message.textContent = 'Неверный ID пользователя';
            return;
        }

        if (!response.ok) {
            message.textContent = 'Не удалось загрузить профиль';
            return;
        }

        const result = await response.json();
        const profile = result.profile;

        if (!profile) {
            message.textContent = 'Сервер не вернул профиль';
            return;
        }

        showProfile(profile);
        message.textContent = '';
        message.hidden = true;
        document.querySelector('#profile-data').hidden = false;

        if (id === 'me') {
            document.querySelector('#logout').hidden = false;
            await loadUnreadArticle(profile);
        }

        await loadProgress(profile);
        await loadLastTask(profile);
    } catch (error) {
        message.textContent = 'Не удалось получить профиль';
        message.hidden = false;
    }
}

async function loadUnreadArticle(profile) {
    let actions = document.querySelector('#unread-article-actions');
    actions.hidden = true;

    let params = new URLSearchParams();
    params.set('page', '1');
    params.set('sortBy', '0');
    params.set('sortDescending', 'false');
    params.set('hideRead', 'true');
    params.set('readArticles', profile.articlesRead || '');

    try {
        const response = await fetch('/api/articles/?' + params.toString());

        if (!response.ok) {
            return;
        }

        const result = await response.json();

        if (!Array.isArray(result.articles) || result.articles.length === 0) {
            return;
        }

        document.querySelector('#unread-article').href = '/article/' + result.articles[0].id;

        actions.hidden = false;
    } catch (error) {
        actions.hidden = true;
    }
}

function showProfile(profile) {
    const easy = profile.easyTasksSolvedCount;
    const medium = profile.mediumTasksSolvedCount;
    const hard = profile.hardTasksSolvedCount;
    const total = easy + medium + hard;

    document.querySelector('#profile-name').textContent = profile.name;
    document.querySelector('#profile-created').textContent = new Date(profile.created).toLocaleDateString('ru-RU');

    document.querySelector('#total-count').textContent = total;
    document.querySelector('#easy-count').textContent = easy;
    document.querySelector('#medium-count').textContent = medium;
    document.querySelector('#hard-count').textContent = hard;

    document.querySelector('#total-bar').style.width = total > 0 ? '100%' : '0%';
    document.querySelector('#easy-bar').style.width = getPercent(easy, total) + '%';
    document.querySelector('#medium-bar').style.width = getPercent(medium, total) + '%';
    document.querySelector('#hard-bar').style.width = getPercent(hard, total) + '%';
}

async function loadProgress(profile) {
    const message = document.querySelector('#statistics-message');

    try {
        const response = await fetch('/api/platform/');

        if (!response.ok) {
            message.textContent = 'Общая статистика временно недоступна';
            message.hidden = false;
            return;
        }

        const result = await response.json();
        const statistics = result.statistics;

        if (!statistics) {
            message.textContent = 'Сервер не вернул общую статистику';
            message.hidden = false;
            return;
        }

        const tasksSolved = profile.easyTasksSolvedCount +
            profile.mediumTasksSolvedCount + profile.hardTasksSolvedCount;

        const tasksCount = statistics.easyTasksCount +
            statistics.mediumTasksCount + statistics.hardTasksCount;

        const articlesPercent = getPercent(profile.articlesReadCount, statistics.articlesCount);
        const tasksPercent = getPercent(tasksSolved, tasksCount);

        showRing('articles', articlesPercent);
        showRing('tasks', tasksPercent);
    } catch (error) {
        message.textContent = 'Не удалось загрузить общую статистику';
        message.hidden = false;
    }
}

async function loadLastTask(profile) {
    const message = document.querySelector('#last-message');
    const details = document.querySelector('#last-details');

    if (!profile.lastTaskSolvedDate) {
        message.textContent = 'Пока нет решённых заданий';
        return;
    }

    document.querySelector('#last-date').textContent = new Date(profile.lastTaskSolvedDate).toLocaleString('ru-RU');
    details.hidden = false;
    message.textContent = '';

    try {
        const response = await fetch('/api/tasks/' + profile.lastTaskSolvedId);

        if (response.status === 404) {
            document.querySelector('#last-name').textContent = 'Задание #' + profile.lastTaskSolvedId + ' не найдено';
            return;
        }

        if (!response.ok) {
            message.textContent = 'Не удалось загрузить задание';
            return;
        }

        const result = await response.json();

        if (!result.task) {
            message.textContent = 'Сервер не вернул задание';
            return;
        }

        document.querySelector('#last-name').textContent = result.task.name;
        document.querySelector('#last-difficulty').textContent = result.task.difficulty;
    } catch (error) {
        message.textContent = 'Не удалось загрузить задание';
    }
}

function showRing(name, percent) {
    const ring = document.querySelector('#' + name + '-ring');
    const text = document.querySelector('#' + name + '-percent');

    if (percent >= 100) {
        ring.style.background = 'var(--color-easy)';
    } else {
        ring.style.background = 'conic-gradient(var(--color-easy) ' + percent + '%, var(--color-white) 0%)';
    }

    text.textContent = percent + '%';
}

function getPercent(value, total) {
    if (total <= 0) {
        return 0;
    }

    const percent = Math.round(value / total * 100);
    return Math.max(0, Math.min(100, percent));
}