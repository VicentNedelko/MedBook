

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
    ref = parseFloat(str);
    console.log('Final - ', ref);
    referenceMin.value = str;
    var referenceMax = document.getElementById('indRefMax');
    referenceMax.value = bear['ReferenceMax'];
    var bearingIndId = document.getElementById('bearingId');
    bearingIndId.value = bear['Id'];

}
