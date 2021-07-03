import { listEnvironments, listEnvironmentInstances, startApplication, stopApplication } from './testingenvironmentsclient.js';

document.addEventListener('DOMContentLoaded', () => {
    init();
}, false);

async function init() {

    await loadDropdown();
    await initList();

    const button = document.querySelector('#startButton');
    button.addEventListener('click', () => startApplicationClick());

    /*setInterval(() => { initList() }, 3000);*/
}

async function loadDropdown() {
    var environments = await listEnvironments();
    const dropdown = document.querySelector('#environmentname');
    for (var env of environments) {
        let option = new Option(env.name, env.name);
        dropdown.add(option);
    }
}

async function initList() {
    const template = document.querySelector('#instancetemplate');
    const parent = document.querySelector('#instancelist');   
    const instances = await listEnvironmentInstances();
    parent.innerHTML = '';
    for (const x of instances) {
        const templateInstatnce = document.importNode(template.content, true);

        templateInstatnce.querySelector('#name').textContent = x.name;   
        templateInstatnce.querySelector('#state').textContent = `(${x.status})`;        

        //link
        const link = templateInstatnce.querySelector('#link');
        link.href = `/endpoint/${x.name}`;

        // Remove button
        const removeButton = templateInstatnce.querySelector('#remove');
        removeButton.addEventListener('click', () => stopApplicationClick(x.name));

        parent.appendChild(templateInstatnce);
    }
}

async function startApplicationClick(name) {
    const dropdown = document.querySelector('#environmentname');
    const value = dropdown.value;
    if (value && value !== '') {
        await startApplication(value);
    }
    
}

async function stopApplicationClick(instanceName) {
    await stopApplication(instanceName);
}