const uriLanguage = 'api/Languages';
let languages = [];

function getLanguages() {
    fetch(uriLanguage)
        .then(response => response.json())
        .then(data => _displayLanguages(data))
        .catch(error => console.error('Неможливо отримати мови.', error));
}

function addLanguage() {
    const addNameTextbox = document.getElementById('add-name-language');

    const language = {
        name: addNameTextbox.value.trim(),
    };

    if (!language.name) {
        showError('Введіть назву мови');
        return;
    }

    fetch(uriLanguage, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(language)
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(errorText => {
                    throw new Error(errorText);
                });
            }
            return response.json();
        })
        .then(() => {
            getLanguages();
            addNameTextbox.value = '';
        })
        .catch(error => {
            showError(error.message);
            console.error('Неможливо створити мову.', error);
        });
}

function deleteLanguage(id) {
    fetch(`${uriLanguage}/${id}`, {
        method: 'DELETE'
    })
        .then(() => {
            getLanguages();
            languages = languages.filter(language => language.id !== id);
            _displayLanguages(languages);
        })
        .catch(error => console.error('Неможливо видалити мову.', error));
}

function displayEditFormLanguage(id) {
    const language = languages.find(language => language.id === id);

    if (language) {
        document.getElementById('edit-id-language').value = language.id || '';
        document.getElementById('edit-name-language').value = language.name || '';
    } else {
        document.getElementById('edit-id-language').value = '';
        document.getElementById('edit-name-language').value = '';
    }
    document.getElementById('editLanguage').style.display = 'block';
}

function updateLanguage() {
    const languageId = document.getElementById('edit-id-language').value;
    const languageName = document.getElementById('edit-name-language').value.trim();

    const language = {
        id: parseInt(languageId, 10),
        name: languageName,
    };

    if (!language.name) {
        showError('Введіть назву мови');
        return;
    }

    fetch(`${uriLanguage}/${languageId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(language)
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(errorText => {
                    throw new Error(errorText);
                });
            }
            getLanguages();
            closeInputLanguage();
        })
        .catch(error => {
            showError(error.message);
            console.error('Неможливо оновити мову.', error);
        });

    return false;
}

function closeInputLanguage() {
    document.getElementById('editLanguage').style.display = 'none';
}

function _displayLanguages(data) {
    const tBody = document.getElementById('languages');
    tBody.innerHTML = '';

    const button = document.createElement('button');

    data.forEach(language => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Редагувати';
        editButton.classList.add('edit-btn', 'btn');
        editButton.setAttribute('onclick', `displayEditFormLanguage(${language.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Видалити';
        deleteButton.classList.add('delete-btn', 'btn');
        deleteButton.setAttribute('onclick', `deleteLanguage(${language.id})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let textNode = document.createTextNode(language.name);
        td1.appendChild(textNode);

        let td2 = tr.insertCell(1);
        let textNodeBooksCount = document.createTextNode(language.books.length);
        td2.appendChild(textNodeBooksCount);

        let td3 = tr.insertCell(2);
        td3.appendChild(editButton);

        let td4 = tr.insertCell(3);
        td4.appendChild(deleteButton);
    });

    languages = data;
}
