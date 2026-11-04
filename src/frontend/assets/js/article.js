const parts = window.location.pathname.split('/');
const articleId = parts[2];
const message = document.querySelector('#article-message');
const content = document.querySelector('#article-content');
const retryActions = document.querySelector('#article-retry-actions');
const readButton = document.querySelector('#read-article');
const readMessage = document.querySelector('#read-message');

let articleReady = false;
let articleRead = false;
let sending = false;
let readArticles = '';

document.querySelector('#article-retry').addEventListener('click', loadArticle);
readButton.addEventListener('click', markAsRead);

loadArticle();

async function loadArticle() {
    articleReady = false;
    articleRead = false;
    content.hidden = true;
    retryActions.hidden = true;
    message.hidden = false;
    message.textContent = 'Загрузка статьи...';
    readButton.disabled = true;
    readButton.textContent = 'Отметить прочитанной';
    readMessage.className = 'article-note';
    readMessage.textContent = 'Загрузка статуса статьи...';

    document.querySelector('#article-login-actions').hidden = true;
    document.querySelector('#next-article-actions').hidden = true;

    const number = Number(articleId);

    if (!articleId || !Number.isInteger(number) || number < 0 || number >= 20000000) {
        message.textContent = 'Некорректный номер статьи';
        return;
    }

    try {
        const response = await fetch('/api/articles/' + articleId);

        if (response.status === 404) {
            message.textContent = 'Статья не найдена';
            return;
        }

        if (!response.ok) {
            message.textContent = 'Что-то пошло не так';
            retryActions.hidden = false;
            return;
        }

        const result = await response.json();

        if (!result.article) {
            message.textContent = 'Что-то пошло не так';
            retryActions.hidden = false;
            return;
        }

        const article = result.article;

        document.querySelector('#article-title').textContent = article.title;
        document.querySelector('#article-topic').textContent = article.topic;
        document.querySelector('#article-created').textContent = new Date(article.createdAt).toLocaleDateString('ru-RU');
        document.title = article.title + ' - 28web';
        renderMarkdown(article.markdown, document.querySelector('#article-markdown'));

        articleReady = true;
        content.hidden = false;
        message.hidden = true;
        await loadReadStatus();
        await loadNextArticle();
    } catch (error) {
        message.textContent = 'Не удалось загрузить статью';
        message.hidden = false;
        retryActions.hidden = false;
    }
}

function renderMarkdown(text, element) {
    const html = marked.parse(text);
    const cleanHtml = DOMPurify.sanitize(html);

    element.innerHTML = cleanHtml;
}

async function loadReadStatus() {
    const token = localStorage.getItem('token');

    if (!token) {
        showLogin();
        return;
    }

    readButton.disabled = false;

    try {
        const response = await fetch('/api/user/profile/me', {
            headers: {
                'Authorization': 'Bearer ' + token
            }
        });

        if (response.status === 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('profileName');
            showLogin();
            return;
        }

        if (!response.ok) {
            readMessage.textContent = 'Не удалось загрузить статус статьи';
            return;
        }

        const result = await response.json();

        if (!result.profile) {
            readMessage.textContent = 'Не удалось загрузить статус статьи';
            return;
        }

        readArticles = result.profile.articlesRead || '';

        if (isRead(Number(articleId))) {
            showRead();
        } else {
            readMessage.textContent = 'Отметьте статью прочитанной, когда закончите читать';
        }
    } catch (error) {
        readMessage.textContent = 'Не удалось загрузить статус статьи';
    }
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

function showLogin() {
    readButton.disabled = true;
    readMessage.textContent = 'Войдите, чтобы сохранить прогресс в профиле';
    document.querySelector('#article-login-actions').hidden = false;
}

function showRead() {
    articleRead = true;
    readButton.disabled = true;
    readButton.textContent = 'Прочитано';
    readMessage.className = 'article-note article-read';
    readMessage.textContent = 'Статья отмечена прочитанной';
}

async function markAsRead() {
    if (!articleReady || articleRead || sending) {
        return;
    }

    const token = localStorage.getItem('token');

    if (!token) {
        showLogin();
        return;
    }

    const article = {
        id: Number(articleId)
    };

    sending = true;
    readButton.disabled = true;
    readMessage.textContent = 'Сохраняем...';

    try {
        const response = await fetch('/api/user/profile/articles', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + token
            },
            body: JSON.stringify(article)
        });

        if (response.status === 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('profileName');
            showLogin();
            return;
        }

        let result;

        try {
            result = await response.json();
        } catch (error) {
            readMessage.textContent = 'Что-то пошло не так';
            return;
        }

        if (response.status === 400 && result.message === 'Article has already been read') {
            showRead();
            return;
        }

        if (!response.ok) {
            readMessage.textContent = 'Что-то пошло не так';
            return;
        }

        showRead();
    } catch (error) {
        readMessage.textContent = 'Что-то пошло не так';
    } finally {
        sending = false;

        if (!articleRead && localStorage.getItem('token')) {
            readButton.disabled = false;
        }
    }
}

async function loadNextArticle() {
    const nextMessage = document.querySelector('#next-message');
    nextMessage.textContent = 'Поиск следующей статьи...';
    let found = false;

    try {
        for (let page = 1; page <= 4000; page += 1) {
            const response = await fetch('/api/articles/?page=' + page + '&sortBy=0&sortDescending=false');

            if (response.status === 404) {
                nextMessage.textContent = 'Следующей статьи пока нет';
                return;
            }

            if (!response.ok) {
                nextMessage.textContent = 'Не удалось загрузить ссылку на статью';
                return;
            }

            const result = await response.json();

            if (!Array.isArray(result.articles)) {
                nextMessage.textContent = 'Не удалось загрузить ссылку на статью';
                return;
            }

            for (const article of result.articles) {
                if (found) {
                    document.querySelector('#next-article').href = '/article/' + article.id;
                    document.querySelector('#next-article-actions').hidden = false;
                    nextMessage.textContent = '';
                    return;
                }

                if (Number(article.id) === Number(articleId)) {
                    found = true;
                }
            }

            if (result.articles.length < 16) {
                nextMessage.textContent = 'Следующей статьи пока нет';
                return;
            }
        }

        nextMessage.textContent = 'Следующей статьи пока нет';
    } catch (error) {
        nextMessage.textContent = 'Не удалось загрузить ссылку на статью';
    }
}