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
})