const parts = window.location.pathname.split('/');
const taskId = parts[2];
const taskMessage = document.querySelector('#task-message');
const taskContent = document.querySelector('#task-content');
const taskRetryActions = document.querySelector('#task-retry-actions');
const flagForm = document.querySelector('#flag-form');
const flagMessage = document.querySelector('#flag-message');
const flagButton = document.querySelector('#flag-submit');
const hintButton = document.querySelector('#show-hint');

let taskReady = false;
let hints = [];
let hintsShown = 0;
let sendingFlag = false;
let solution = '';

flagForm.addEventListener('submit', sendFlag);
hintButton.addEventListener('click', showHint);
document.querySelector('#task-retry').addEventListener('click', loadTask);
document.querySelector('#instruction-retry').addEventListener('click', loadInstructions);
document.querySelector('#show-solution').addEventListener('click', showSolution);

loadTask();

async function loadTask() {
    taskReady = false;
    taskContent.hidden = true;
    taskRetryActions.hidden = true;
    taskMessage.hidden = false;
    taskMessage.textContent = 'Загрузка задания...';

    const number = Number(taskId);

    if (!taskId || !Number.isInteger(number) || number < 0 || number >= 2000000) {
        taskMessage.textContent = 'Неверный номер задания';
        return;
    }

    try {
        const response = await fetch('/api/tasks/' + taskId);

        if (response.status === 404) {
            taskMessage.textContent = 'Задание не найдено';
            return;
        }

        if (!response.ok) {
            taskMessage.textContent = 'Не удалось загрузить задание';
            taskRetryActions.hidden = false;
            return;
        }

        const result = await response.json();

        if (!result.task) {
            taskMessage.textContent = 'Что-то пошло не так';
            taskRetryActions.hidden = false;
            return;
        }

        showTask(result.task);
        taskReady = true;
        taskContent.hidden = false;
        taskMessage.hidden = true;
        await loadInstructions();
        await loadTaskStatus();
    } catch (error) {
        taskMessage.textContent = 'Что-то пошло не так';
        taskMessage.hidden = false;
        taskRetryActions.hidden = false;
    }
}

function showTask(task) {
    document.querySelector('#task-name').textContent = task.name;
    document.querySelector('#task-created').textContent = new Date(task.created).toLocaleDateString('ru-RU');
    document.querySelector('#task-description').textContent = task.description;
    document.title = task.name + ' - 28web';

    let difficulty = document.querySelector('#task-difficulty');
    difficulty.textContent = task.difficulty;

    difficulty.className = 'task-difficulty task-difficulty-' + task.difficulty.trim().toLowerCase();

    let download = document.querySelector('#task-download');
    let downloadActions = document.querySelector('#task-download-actions');
    let fileMessage = document.querySelector('#task-file-message');

    if (task.file) {
        download.href = '/api/files/' + task.file;

        downloadActions.hidden = false;
        fileMessage.hidden = true;
    } else {
        downloadActions.hidden = true;
        fileMessage.hidden = false;
    }

    hints = Array.isArray(task.hints) ? task.hints : [];
    solution = typeof task.solution === 'string' ? task.solution.trim() : '';
    hintsShown = 0;
    document.querySelector('#task-hints').textContent = '';
    document.querySelector('#task-hints-panel').hidden = hints.length === 0 && solution === '';
    document.querySelector('#task-hint-actions').hidden = hints.length === 0;
    document.querySelector('#task-hints-title').textContent = hints.length === 0 ? 'Решение' : 'Подсказки';
    document.querySelector('#task-solution-actions').hidden = hints.length > 0 || solution === '';
    document.querySelector('#task-solution').hidden = true;
    document.querySelector('#task-solution').textContent = '';
    hintButton.disabled = false;
    hintButton.textContent = 'Открыть подсказку';
}

async function loadTaskStatus() {
    const token = localStorage.getItem('token');

    if (!token) {
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
            return;
        }

        if (!response.ok) {
            return;
        }

        const result = await response.json();

        if (!result.profile) {
            return;
        }

        document.querySelector('#task-save-note').hidden = true;
        let solved = result.profile.tasksSolved || '';

        let parts = solved.split(',');

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

            if (Number(taskId) >= first && Number(taskId) <= last) {
                document.querySelector('#task-solved').hidden = false;
                return;
            }
        }
    } catch (error) {
        return;
    }
}

async function sendFlag(event) {
    event.preventDefault();

    if (!taskReady || sendingFlag) {
        return;
    }

    const flag = flagForm.elements.flag.value.trim();

    if (flag === '') {
        flagMessage.hidden = false;
        flagMessage.className = 'flag-message';
        flagMessage.textContent = 'Введите флаг';
        return;
    }

    const token = localStorage.getItem('token');
    const headers = {
        'Content-Type': 'application/json'
    };

    if (token) {
        headers.Authorization = 'Bearer ' + token;
    }

    sendingFlag = true;
    flagButton.disabled = true;
    flagMessage.hidden = false;
    flagMessage.className = 'flag-message';
    flagMessage.textContent = 'Проверяем флаг...';

    try {
        const response = await fetch('/api/tasks/' + taskId + '/solve', {
            method: 'POST',
            headers: headers,
            body: JSON.stringify({flag: flag})
        });

        const result = await response.json();

        if (response.status === 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('profileName');
            flagMessage.textContent = 'Войдите в аккаунт и повторите отправку';
            document.querySelector('#task-save-note').hidden = false;
            return;
        }

        if (response.status === 404) {
            flagMessage.textContent = 'Задание не найдено';
            return;
        }

        if (response.status === 400) {
            if (result.message === 'Wrong flag') {
                flagMessage.textContent = 'Флаг не подошёл =(';
            } else {
                flagMessage.textContent = 'Не удалось проверить флаг';
            }
            return;
        }

        if (!response.ok) {
            flagMessage.textContent = 'Что-то пошло не так';
            return;
        }

        flagMessage.className = 'flag-message flag-success';
        flagMessage.textContent = 'Флаг принят!';
        flagForm.elements.flag.value = '';

        if (token) {
            await loadTaskStatus();
        }
    } catch (error) {
        flagMessage.textContent = 'Что-то пошло не так';
    } finally {
        sendingFlag = false;
        flagButton.disabled = false;
    }
}

function showHint() {
    if (hintsShown >= hints.length) {
        return;
    }

    let hint = document.createElement('p');
    hint.className = 'task-hint';
    hint.textContent = 'Подсказка ' + (hintsShown + 1) + ': ' + hints[hintsShown];
    document.querySelector('#task-hints').append(hint);
    hintsShown += 1;

    if (hintsShown === hints.length) {
        hintButton.disabled = true;
        hintButton.textContent = 'Все подсказки открыты';
        document.querySelector('#task-solution-actions').hidden = solution === '';
    } else {
        hintButton.textContent = 'Следующая подсказка';
    }
}

function renderMarkdown(text, element) {
    let html = marked.parse(text);
    let cleanHtml = DOMPurify.sanitize(html);

    element.innerHTML = cleanHtml;
}

async function loadInstructions() {
    let instructions = document.querySelector('#task-instructions');
    let retry = document.querySelector('#instruction-retry-actions');
    instructions.textContent = 'Загрузка инструкции...';
    retry.hidden = true;

    try {
        let response = await fetch('/assets/md/task-instructions.md');

        if (!response.ok) {
            instructions.textContent = 'Не удалось загрузить инструкцию';
            retry.hidden = false;
            return;
        }

        let text = await response.text();
        renderMarkdown(text, instructions);
    } catch (error) {
        instructions.textContent = 'Не удалось загрузить инструкцию';
        retry.hidden = false;
    }
}

function showSolution() {
    if (hintsShown < hints.length || solution === '') {
        return;
    }

    let content = document.querySelector('#task-solution');
    renderMarkdown(solution, content);
    content.hidden = false;
    document.querySelector('#task-solution-actions').hidden = true;
}
