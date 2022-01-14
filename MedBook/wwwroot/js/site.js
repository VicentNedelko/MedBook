

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

function FillUnitValue() {
    var actualIndicatorJson = document.getElementById('indJsonData').value;
    console.log(actualIndicatorJson);
    var indicator = JSON.parse(actualIndicatorJson);
    var unitValue = document.getElementById('unitValue');
    unitValue.value = indicator['Unit'];
    console.log('Units - ', unitValue.value);
}
