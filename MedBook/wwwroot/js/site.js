﻿

function FillInputs() {
    var actualBearingJson = document.getElementById('bearingIndicatorId').value;
    var bear = JSON.parse(actualBearingJson);
    console.log(bear);
    var unit = document.getElementById('indUnit');
    unit.value = bear['Unit'];
    var referenceMin = document.getElementById('indRefMin');
    var ref = bear['ReferenceMin'];
    console.log('Before - ', ref);
    var str = ref.toString();
    str = str.replace('.', ',');
    console.log('After - ', str);
    referenceMin.value = str;
    var referenceMax = document.getElementById('indRefMax');
    strMax = bear['ReferenceMax'].toString();
    strMax = strMax.replace('.', ',');
    referenceMax.value = strMax;
    console.log('RefMAX - ', strMax);
    var bearingIndId = document.getElementById('bearingId');
    bearingIndId.value = bear['Id'];

}

function GetUserBlockStatus() {
    var selectorValue = document.getElementById('isBlockSelector').value;
    console.log(selectorValue);
    var isBlockInputValue = document.getElementById('isBlock');
    isBlockInputValue.value = selectorValue;
    if (selectorValue === 'true') {
        const myModal = new bootstrap.Modal(document.getElementById('staticBackdrop'), { backdrop: 'static' });
        var userInfo = document.getElementById('userInfo');
        var userLabel = document.getElementById('staticBackdropLabel');
        var LName = document.getElementById('userLName').value;
        var FName = document.getElementById('userFName').value;
        userInfo.innerHTML = FName + ' ' + LName + ' заблокирован!';
        userLabel.innerHTML = 'Блокировка пользователя';
        myModal.show();
    }
}

function ShowModalIndDuplication(infoData) {
    var indName = document.getElementById('indName').value;
    console.log(indName);
    const myModal = new bootstrap.Modal(document.getElementById('indDuplication'), { backdrop: 'static' });
    var indLabel = document.getElementById('indDuplicationLabel');
    var indInfo = document.getElementById('indInfo');
    indLabel.innerHTML = infoData.toString();
    indInfo.innerHTML = indName;
    myModal.show();
}

function FillUnitValue() {
    var actualIndicatorJson = document.getElementById('indJsonData').value;
    console.log(actualIndicatorJson);
    var indicator = JSON.parse(actualIndicatorJson);
    var unitValue = document.getElementById('unitValue');
    unitValue.value = indicator['Unit'];
    console.log('Units - ', unitValue.value);
}

function GetBearingIndicator() {
    var chosenBearingIndicator = document.getElementById('bearingIndicators').value;
    var indicator = JSON.parse(chosenBearingIndicator);
    console.log(indicator);
    var modelBearingIndicatorId = document.getElementById('modelBearingIndicatorId');
    modelBearingIndicatorId.value = indicator['Id'];
    var modelIndicatorUnit = document.getElementById('unit');
    modelIndicatorUnit.value = indicator['Unit'];

    var modelIndicatorMin = document.getElementById('referentMin');
    var strMin = indicator['ReferenceMin'].toString();
    strMin = strMin.replace('.', ',');
    modelIndicatorMin.value = strMin;

    var modelIndicatorMax = document.getElementById('referentMax');
    var strMax = indicator['ReferenceMax'].toString();
    strMax = strMax.replace('.', ',');
    modelIndicatorMax.value = strMax;
}

function FillNonDigitReference() {
    var indicatorType = document.getElementById('indicatorType').value;
    var refMin = document.getElementById('referenceMin');
    var refMax = document.getElementById('referenceMax');
    if (indicatorType == 1) {
        refMin.value = -1;
        refMax.value = -1;
    }
    else {
        refMin.value = '';
        refMax.value = '';
    }
}

function displayToastOnEmptyResearch() {
    var ResearchToast = document.getElementById('liveToast');
    const emptyResearchToast = new bootstrap.Toast(ResearchToast);
    emptyResearchToast.show();
}

function GetFileName() {
    var fileInput = document.getElementById('fileUpload').value;
    var pathElements = fileInput.split("\\");
    var fileName = pathElements[pathElements.length - 1];
    var fileUploadName = document.getElementById('fileUploadName');
    fileUploadName.innerHTML = fileName;
}

function manualIndicatorSelected() {
    var radioButtons = document.querySelectorAll('input[name = "sampleIndicator"]');
    console.log(radioButtons);
    for (var i = 0; i < radioButtons.length; i++) {
        if (radioButtons[i].checked) {
            console.log('2');
            var jsonValue = radioButtons[i].value;
            console.log('Selected - ', radioButtons[i].value);
            var chosenIndicator = JSON.parse(jsonValue);
            break;
        }
    }
    console.log(jsonValue);
    const manualIndicatorModal = new bootstrap.Modal(document.getElementById('manualIndicatorValueAdd'), { keyboard: true, backdrop: 'static' });
    if (chosenIndicator['Type'] == 0) {
        document.getElementById('manualIndicatorValueContainer').innerHTML = '<input id="manualIndicatorValue" class="form-control col-lg-4"  name="manualIndicatorValue" required/>';
        document.getElementById('manualIndicatorValue').focus();
    }
    else {
        document.getElementById('manualIndicatorValueContainer').innerHTML = '<select class="form-control" id="manualIndicatorValue" name="manualIndicatorValue"><option value ="0"> Не обнаружено</option ><option value="1">Обнаружено</option></select > ';
    }
    document.getElementById('manualIndicatorVariants').hidden = false;
    document.getElementById('manualIndicatorTable').hidden = false;
    document.getElementById('manualIndicatorLabel').hidden = false;
    document.getElementById('manualIndicatorHR').hidden = false;
    document.getElementById('addIndicatorBtn').hidden = false;
    document.getElementById('manualIndicatorName').value = chosenIndicator['Name'];
    document.getElementById('manualIndicatorUnit').value = chosenIndicator['Unit'];
    $('#manualIndicatorMAX').val(chosenIndicator['ReferentMax']);
    $('#manualIndicatorMIN').val(chosenIndicator['ReferentMin']);
    $('#indicatorId').val(chosenIndicator['Id']);
    document.getElementById('addIndicatorBtn').hidden = false;
    manualIndicatorModal.show();
}

function editAddIndicatorSelected() {
    var radios = document.querySelectorAll('input[name="addSampleIndicator"]');
    console.log(radios);
    for (var i = 0; i < radios.length; i++) {
        if (radios[i].checked) {
            var jsonIndicator = radios[i].value;
            var indicatorToAdd = JSON.parse(jsonIndicator);
            console.log('Selected - ', radios[i].value);
            break;
        }
    }
    console.log('Indicator - ', indicatorToAdd);
    if (indicatorToAdd['Type'] == 0) {
        document.getElementById('addIndicatorValueContainer').innerHTML = '<input id="addIndicatorValue" class="form-control col-lg-4"  name="addIndicatorValue" required/>';
        document.getElementById('addIndicatorValue').focus();
    }
    else {
        document.getElementById('addIndicatorValueContainer').innerHTML = '<select class="form-control" id="addIndicatorValue" name="addIndicatorValue"><option value ="0"> Не обнаружено</option ><option value="1">Обнаружено</option></select > ';
    }
    $('#addIndicatorName').val(indicatorToAdd['Name']);
    $('#addIndicatorUnit').val(indicatorToAdd['Unit']);
    $('#addIndicatorMAX').val(indicatorToAdd['ReferentMax']);
    $('#addIndicatorMIN').val(indicatorToAdd['ReferentMin']);
    $('#bearingId').val(indicatorToAdd['BearingIndicatorId']);
    document.getElementById('addIndicatorButton').hidden = false;
    document.getElementById('indicatorToAddToEditResearch').hidden = false;
}

function hidePartialViews() {
    var valueTag = document.getElementById('manualIndicatorValue');
    if (valueTag.tagName.toLowerCase() === 'input') {
        console.log('TAG - ', valueTag.tagName.toLowerCase());
        if (valueTag.value.length != 0) {
            document.getElementById('indicatorSearchList').hidden = true;
        }
    }
    if (valueTag.tagName.toLowerCase() === 'select') {
        console.log('TAG - ', valueTag.tagName.toLowerCase());
        document.getElementById('indicatorSearchList').hidden = true;
    }
    document.getElementById('addIndicatorBtn').hidden = true;
    document.getElementById('manualResearchHR').hidden = true;
}

function showIndicatorList() {
    document.getElementById('indicatorSearchList').hidden = false;
    document.getElementById('manualIndicatorVariants').hidden = true;
    document.getElementById('manualIndicatorName').value = '';
    var indicatorValueTag = document.getElementById('manualIndicatorValue');
    if (!!indicatorValueTag) {
        indicatorValueTag.value = '';
    }
    document.getElementById('manualIndicatorUnit').value = '';
}

function ShowAddIndicatorForEdit() {
    const findModal = new bootstrap.Modal(document.getElementById('FindIndicatorOnEdit'), { keyboard: true, backdrop: 'static' });
    findModal.show();
}

function HideAddIndicator() {
    document.getElementById('addIndicatorButton').hidden = true;
}

function NotifyDoctor() {
    var flag = document.getElementById('notificateDoctor');
    console.log('flag - ', flag);
    if (flag.checked) {
        flag.value = true;
        console.log('true');
    }
    else {
        flag.value = false;
        console.log('false');
    }
}
