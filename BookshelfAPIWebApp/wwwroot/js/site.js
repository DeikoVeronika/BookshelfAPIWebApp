document.addEventListener('DOMContentLoaded', function () {
    const showFormButton = document.getElementById('show-create-category-form');
    const categoryForm = document.querySelector('.create-category-form');
    const button = document.querySelector('.btn');

    showFormButton.addEventListener('click', function () {
        if (categoryForm.style.display === 'none') {
            categoryForm.style.display = 'block';
            button.classList.add('create-page-btn-active');
        } else {
            categoryForm.style.display = 'none';
            button.classList.remove('create-page-btn-active');
        }
    });
});

document.addEventListener('DOMContentLoaded', function () {
    const showFormButton = document.getElementById('show-create-author-form');
    const authorForm = document.querySelector('.create-author-form');

    showFormButton.addEventListener('click', function () {
        if (authorForm.style.display === 'none') {
            authorForm.style.display = 'block';
        } else {
            authorForm.style.display = 'none';
        }
    });
});

