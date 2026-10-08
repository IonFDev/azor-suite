const API_URL = 'http://127.0.0.1:8082/api';

const token = localStorage.getItem('token');

$(document).ready(function () {

    $.ajax({
        url: `${API_URL}/user`,
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Authorization': `Bearer ${token}`
        },

        success: function (user) {
            const userLogo = $('#log-out-icon-text');
            userLogo.text(
                user.name.charAt(0).toUpperCase() +
                user.surname.charAt(0).toUpperCase()
            );

            if (user.role === 'admin') {
                const adminButton = $('<button>')
                    .text('+ Agregar Listado')
                    .addClass('btn-add-list')
                    .attr('onclick', 'window.location.href = "create.html"');

                $('#lists-container').append(adminButton);
            }

        }

    });

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