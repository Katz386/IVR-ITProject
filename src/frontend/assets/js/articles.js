const filters = document.querySelector('#articles-filters');
const rows = document.querySelector('#articles-rows');
const state = document.querySelector('#articles-state');
const message = document.querySelector('#articles-message');
const retry = document.querySelector('#retry-articles');
const retryActions = document.querySelector('#retry-actions');
const previous = document.querySelector('#previous-page');
const next = document.querySelector('#next-page');
const pageNumber = document.querySelector('#page-number');

let page = 1;
let loading = false;
let canNext = false;
let profileLoaded = false;
let readArticles = '';

filters.addEventListener('submit', applyFilters);
filters.elements.sort.addEventListener('change', applyFilters);
filters.elements.hideRead.addEventListener('change', applyFilters);

document.querySelector('#reset-filters').addEventListener('click', function () {
    filters.reset();
    page = 1;
    loadArticles();
});

previous.addEventListener('click', function () {
    if (loading || page <= 1) {
        return;
    }

    page -= 1;
    loadArticles();
});

next.addEventListener('click', function () {
    if (loading || !canNext) {
        return;
    }

    page += 1;
    loadArticles();
});

retry.addEventListener('click', loadArticles);

startArticles();

async function startArticles() {
    setBusy(true);
    await loadReadArticles();
    await loadArticles();
}

function applyFilters(event) {
    event.preventDefault();

    if (loading) {
        return;
    }

    page = 1;
    loadArticles();
}

async function loadReadArticles() {
    const token = localStorage.getItem('token');

    if (!token) {
        filters.elements.hideRead.title = 'Войдите, чтобы видеть прочитанные статьи';
        return;
    }

    try {
        const response = await fetch('/api/user/profile/me', {
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });

        if (response.status === 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('profileName');
            filters.elements.hideRead.title = 'Сессия истекла';
            return;
        }

        if (!response.ok) {
            filters.elements.hideRead.title = 'Что-то пошло не так';
            return;
        }

        const result = await response.json();

        if (!result.profile) {
            filters.elements.hideRead.title = 'Что-то пошло не так';
            return;
        }

        readArticles = result.profile.articlesRead || '';
        profileLoaded = true;
        filters.elements.hideRead.title = 'Скрыть прочитанные статьи';
    } catch (error) {
        filters.elements.hideRead.title = 'Что-то пошло не так';
    }
}

async function loadArticles() {
    setBusy(true);
    rows.textContent = '';
    state.hidden = false;
    retryActions.hidden = true;
    message.textContent = 'Загрузка статей...';
    pageNumber.textContent = 'Страница ' + page;
    canNext = false;

    const parameters = new URLSearchParams();
    parameters.set('page', page);

    const search = filters.elements.search.value.trim();
    const topic = filters.elements.topic.value.trim();
    const sort = filters.elements.sort.value;

    if (search !== '') {
        parameters.set('search', search);
    }

    if (topic !== '') {
        parameters.set('topic', topic);
    }

    if (sort === 'order') {
        parameters.set('sortBy', '0');
        parameters.set('sortDescending', 'false');
    } else {
        parameters.set('sortBy', '1');
        parameters.set('sortDescending', sort !== 'oldest');
    }

    if (profileLoaded && filters.elements.hideRead.checked) {
        parameters.set('hideRead', 'true');
        parameters.set('readArticles', readArticles);
    }

    try {
        const response = await fetch('/api/articles/?' + parameters.toString());

        if (response.status === 404) {
            showEmpty();
            return;
        }

        if (response.status === 400) {
            message.textContent = 'Некорректные фильтры';
            return;
        }

        if (!response.ok) {
            message.textContent = 'Не удалось загрузить статьи';
            retryActions.hidden = false;
            return;
        }

        const result = await response.json();

        if (!Array.isArray(result.articles)) {
            message.textContent = 'Ошибка на стороне сервера';
            retryActions.hidden = false;
            return;
        }

        if (result.articles.length === 0) {
            showEmpty();
            return;
        }

        for (const article of result.articles) {
            addArticle(article);
        }

        canNext = result.articles.length === 16 && page < 4000;
        state.hidden = true;
    } catch (error) {
        message.textContent = 'Что-то пошло не так';
        retryActions.hidden = false;
    } finally {
        setBusy(false);
    }
}

function addArticle(article) {
    const link = document.createElement('a');
    link.className = 'article-row';
    link.href = '/article/' + article.id;

    if (profileLoaded && isRead(article.id)) {
        link.className = 'article-row article-row-read';
    }

    const name = document.createElement('span');
    name.className = 'article-name';
    name.textContent = article.id + '. ' + article.title;

    const topic = document.createElement('span');
    topic.className = 'article-topic';
    topic.textContent = article.topic;

    link.append(name, topic);
    rows.append(link);
}

function isRead(id) {
    if (readArticles === '') {
        return false;
    }

    let parts = readArticles.split(',');

    for (let part of parts) {
        if (part === '') {
            continue;
        }

        let range = part.split('-');
        let first = Number(range[0]);
        let last = first;

        if (range.length === 2) {
            last = Number(range[1]);
        }

        if (id >= first && id <= last) {
            return true;
        }
    }

    return false;
}

function showEmpty() {

    if (page > 1) {
        message.textContent = 'На этой странице нет статей. Так вышло...';
    } else {
        message.textContent = 'Статьи не найдены';
    }
}

function setBusy(value) {
    loading = value;

    for (const field of filters.elements) {
        field.disabled = value;
    }

    filters.elements.hideRead.disabled = value || !profileLoaded;
    previous.disabled = value || page <= 1;
    next.disabled = value || !canNext;
    retry.disabled = value;
}