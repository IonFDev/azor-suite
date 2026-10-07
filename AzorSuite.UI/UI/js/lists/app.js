const API_URL = 'http://localhost:8080/api';

const token = localStorage.getItem('token');
const user = JSON.parse(localStorage.getItem('user'));

$(document).ready(function () {

    const userLogo = $('#log-out-icon-text');

    if (user) {
        userLogo.text(
            user.name.charAt(0).toUpperCase() +
            user.surname.charAt(0).toUpperCase()
        );
    }

    $.ajax({
        url: `${API_URL}/listados`,
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        success: function (data) {

            const list = $('<ul>');

            data.forEach(function (listado) {

                const listItem = $('<li>');

                const itemName = $('<label>')
                    .text(listado.name);

                const btnGenerateItem = $('<button>')
                    .text('Generar')
                    .addClass('btn-generate');

                const btnUseItem = $('<button>')
                    .text('¿Qué hace?')
                    .addClass('btn-use');

                listItem.append(
                    itemName,
                    btnGenerateItem,
                    btnUseItem
                );

                list.append(listItem);
            });

            $('#lists-container').append(list);
        },
        error: function (xhr, status, error) {
            console.error('Error fetching listados:', error);
            window.location.href = '../errors/404-error.html';
        }
    });

});