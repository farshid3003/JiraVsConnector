optionreader = function() { }

optionreader.getselectedtext = function(id) {
    var sel = document.getElementById(id);
    var opt = sel.options[sel.selectedIndex];
    return opt.text;
}

optionreader.getselectedval = function(id) {
    var sel = document.getElementById(id);
    var opt = sel.options[sel.selectedIndex];
    return opt.value;
}

optionreader.setselectedval = function(id, val) {
    for(i = 0; i < document.all(id).length; i++) {
        if(document.all(id).options[i].value == val) {
            document.all(id).selectedIndex = i;
        }
    }
}

optionreader.clearoptions = function(id) {
    removeAllOptions(document.getElementById(id));
}

optionreader.addoption = function(id, val, name) {
    var optn = document.createElement("OPTION");
    optn.text = name;
    optn.value = val;
    document.getElementById(id).options.add(optn);
}

function removeAllOptions(selectbox) {
    var i;
    for(i = selectbox.options.length - 1; i >= 0; i--) {
        selectbox.remove(i);
    }
}


