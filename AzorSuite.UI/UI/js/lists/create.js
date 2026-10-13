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

            if (user.role !== 'admin') {
                window.location.href = '../dashboard/index.html';
            }

            const userLogo = $('#log-out-icon-text');
            userLogo.text(
                user.name.charAt(0).toUpperCase() +
                user.surname.charAt(0).toUpperCase()
            );

        },

        error: function () {
            window.location.href = '../dashboard/index.html';
        }

    });

    $('#createListForm').on('submit', function (e) {
        e.preventDefault();

        const formData = new FormData();

        formData.append(
            'name',
            $('#listName').val()
        );

        formData.append(
            'description',
            $('#listDescription').val()
        );

        const image = $('#listImage')[0].files[0];

        if (image) {
            formData.append('image', image);
        }

        $.ajax({
            url: `${API_URL}/listados`,
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Authorization': `Bearer ${token}`
            },

            data: formData,
            processData: false,
            contentType: false,

            success: function (response) {
                
                window.location.href = '../lists/index.html';
            },

            error: function (xhr) {
                console.error(
                    'Error creando listado:',
                    xhr.responseJSON
                );
            },                    
        });
    });

});