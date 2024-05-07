const uri = 'api/Categories';
let categories = [];

function showError(message) {
    Swal.fire({
        icon: 'error',
        title: 'Помилка',
        text: message,
    });
}

function getCategories() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayCategories(data))
        .catch(error => console.error('Неможливо отримати категорію.', error));
}

function addCategory() {
    const addNameTextbox = document.getElementById('add-name-category');
    const addDescriptionTextbox = document.getElementById('add-description-category');

    const category = {
        name: addNameTextbox.value.trim(),
        description: addDescriptionTextbox.value.trim(),
    };

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(category)
    })
        .then(response => {
            if (!response.ok) {
                return response.text().then(errorText => {
                    throw new Error(`Неможливо створити категорію. Категорія з такою назвою вже існує.`);
                });
            }
            return response.json();
        })
        .then(() => {
            getCategories();
            addNameTextbox.value = '';
            addDescriptionTextbox.value = '';
        })
        .catch(error => {
            showError(error.message);
            console.error('Неможливо створити категорію.', error);
        });
}
function deleteCategory(id) {
    fetch(`${uri}/${id}`, {
        method: 'DELETE'
    })
        .then(() => {
            getCategories();
            categories = categories.filter(category => category.id !== id);
            _displayCategories(categories);
        })
        .catch(error => console.error('Неможливо видалити категорію.', error));
}


function displayEditForm(id) {
    document.getElementById('edit-id-category').value = '';
    document.getElementById('edit-name-category').value = '';
    document.getElementById('edit-desctiption-category').value = '';

    const category = categories.find(category => category.id === id);
    if (category) {
        document.getElementById('edit-id-category').value = category.id;
        document.getElementById('edit-name-category').value = category.name;
        document.getElementById('edit-desctiption-category').value = category.description;
    }

    document.getElementById('editCategory').style.display = 'block';
}




function updateCategory() {
    const categoryId = document.getElementById('edit-id-category').value; 
    const categoryName = document.getElementById('edit-name-category').value.trim();
    const categoryDescription = document.getElementById('edit-desctiption-category').value.trim(); 

    const category = {
        id: parseInt(categoryId, 10),
        name: categoryName,
        description: categoryDescription
    };

    fetch(`${uri}/${categoryId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(category)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Неможливо оновити категорію. Категорія з такою назвою вже існує.');
            }
            getCategories();
            closeInput();
        })
        .catch(error => {
            showError(error.message);
            console.error('Неможливо оновити категорію.', error);
        });

    return false;
}




function closeInput() {
    document.getElementById('editCategory').style.display = 'none';
}

function _displayCategories(data) {
    const tBody = document.getElementById('categories');
    tBody.innerHTML = '';

    const button = document.createElement('button');

    data.forEach(category => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Редагувати';
        editButton.classList.add('edit-btn', 'btn');

        editButton.addEventListener('click', () => {
            displayEditForm(category.id);
        });

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Видалити';
        deleteButton.classList.add('delete-btn', 'btn');
        deleteButton.setAttribute('onclick', `deleteCategory(${category.id})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let textNode = document.createTextNode(category.name);
        td1.appendChild(textNode);

        let td2 = tr.insertCell(1);
        let textNodeDescription = document.createTextNode(category.description);
        td2.appendChild(textNodeDescription);

        let td3 = tr.insertCell(2);
        td3.appendChild(editButton);

        let td4 = tr.insertCell(3);
        td4.appendChild(deleteButton);
    });

    categories = data;
}
