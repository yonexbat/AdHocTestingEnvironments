import { listEnvironments, listEnvironmentInstances, startApplication, stopApplication } from './testingenvironmentsclient.js';

document.addEventListener('DOMContentLoaded', () => {
    init();
}, false);

async function init() {

    await loadDropdown();
    await initList();

    const button = document.querySelector('#startButton');
    button.addEventListener('click', () => startApplicationClick());

    setInterval(() => { initList() }, 30000);
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

        templateInstatnce.querySelector('#info').textContent = `${x.name} (state: ${x.status}, starttime: ${x.startTime}, planned uptime: ${x.numHoursToRun})`;         

        // link
        const link = templateInstatnce.querySelector('#link');
        link.href = `/endpoint/${x.name}`;

        // Remove button
        const removeButton = templateInstatnce.querySelector('#remove');
        removeButton.addEventListener('click', () => stopApplicationClick(x.name));

        parent.appendChild(templateInstatnce);
    }
}

async function startApplicationClick(name) {
    const applicationName = document.querySelector('#environmentname').value;
    const numhours = document.querySelector('#numhoursrunning').value;
    const numHoursInt = parseInt(numhours);

    if (applicationName && applicationName !== '' && numHoursInt > 0) {
        await startApplication(applicationName, numHoursInt);
    }
    await initList();
}

async function stopApplicationClick(instanceName) {
    await stopApplication(instanceName);
    await initList();
}