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
        }
    });
});