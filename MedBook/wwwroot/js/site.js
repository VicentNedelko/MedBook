// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

'use strict';

const saveBtn = document.querySelector('#saveIndicatorList');
saveBtn.addEventListener('click', SaveIndicatorList);

function SaveIndicatorList() {
    const items = document.querySelectorAll('.indicator-item');
    items.forEach(function (item) {
        console.log(JSON.stringify(item.innerHTML, null, 2));
    });
};