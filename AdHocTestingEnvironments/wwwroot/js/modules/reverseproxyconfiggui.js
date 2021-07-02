import { getRoutes, createRoute, deleteRoute } from './reverseproxyconfigclient.js'

document.addEventListener('DOMContentLoaded', () => {
    init();
}, false);

async function init() {
   
    const newRoute = { name: 'abc', destination: 'https://www.sbb.ch' };
    await createRoute(newRoute);
    await initList();
    const button = document.querySelector('#addRoute');
    button.addEventListener('click', () => addRoute());
}


async function initList() {
    const template = document.querySelector('#routetemplate');
    const parent = document.querySelector('#routelist');
    parent.innerHTML = '';
    const routes = await getRoutes();
    for (const x of routes) {
        const templateInstatnce = document.importNode(template.content, true);

        templateInstatnce.querySelector('#name').textContent = x.name;
        templateInstatnce.querySelector('#destination').textContent = x.destination;
        const removeButton = templateInstatnce.querySelector('#remove');
        removeButton.addEventListener('click', () => removeRoute(x.name));

        parent.appendChild(templateInstatnce);
    }
}

async function addRoute() {
    const route = document.querySelector('#routeName').value;
    const destination = document.querySelector('#routeDestination').value;
    const newRoute = { name: route, destination: destination };
    await createRoute(newRoute);
    await initList();
}

async function removeRoute(name) {
    await deleteRoute(name);
    await initList();
}