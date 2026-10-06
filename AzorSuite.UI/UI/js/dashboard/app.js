const token = localStorage.getItem('token');
const user = JSON.parse(localStorage.getItem('user'));

const resultado = document.getElementById('result');
resultado.textContent = `Bienvenido, ${user.name} ${user.surname}`;