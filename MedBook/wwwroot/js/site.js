

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
    var isBlockInputValue = document.getElementById('isBlock');
    var toast = document.getElementById('warningToast');
    toast.toast("show");
    isBlockInputValue.value = selectorValue;
    console.log('Is Block = ', selectorValue);
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
