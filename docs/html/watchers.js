$(document).ready(function() {
  watcherPropertyNameChange();
  watcherPropertyPrimitiveChange();
  watcherValidationDependsOnChange();
  watcherRemovePropertyChange();
});

function watcherRemovePropertyChange() {
  $(document).on("click", ".remove-property", function(element) {
    var nameProperty = $(element.target).parent().parent().find(".property-name").val();

    var properties = $(element.target).parent().parent().parent().parent();

    $.each($(properties).find(".validation-depends-on"), function(index, validationDependsOn) {
      var nameDependsOn = $(validationDependsOn).val();

      if (nameDependsOn !== nameProperty ||
          nameDependsOn === "") {
        return;
      }

      var validationDependsWhen = $(validationDependsOn).parent().parent().find(".validation-depends-when");

      clearSelect(validationDependsOn);
      clearSelect(validationDependsWhen);
    });

    $(element.target).parent().parent().parent().remove();
  });
}

function watcherPropertyNameChange() {
  $(document).on("blur", ".property-name", function(element) {
    var entity = $(element.target).parent().parent().parent().parent().parent();

    var values = getPropertyNameValues(entity);

    $.each($(entity).find(".validation-depends-on"), function(index, validationDependsOn) {
      var oldValue = $(validationDependsOn).val();

      var exists = false;

      if ($.inArray($(validationDependsOn).val(), values) > -1) {
        exists = true;
      }

      clearSelect(validationDependsOn);

      $.each(values, function(index, value) {
        $(validationDependsOn).append(`<option value="${value}">${value}</option>`);
      });

      if (exists) {
        $(validationDependsOn).val(oldValue);
      } else {
        var validationDependsWhen = $(validationDependsOn).parent().parent().find(".validation-depends-when");

        clearSelect(validationDependsWhen);
      }
    });
  });
}

function watcherPropertyPrimitiveChange() {
  $(document).on("change", ".property-primitive", function(propertyPrimitive) {
    var property = $(propertyPrimitive.target).parent().parent().parent();

    var primitive = $(property).find('select[class="property-primitive"]').val();

    $.each($(property).find(".validation-type"), function(index, validationType) {
      clearSelect(validationType);

      if (!primitive) {
        return;
      }

      var validations = getValidations(primitive);

      $.each(validations, function(index, value) {
        $(validationType).append(`<option value="${value}">${value}</option>`);
      });
    });

    var properties = $(property).parent();

    var nameProperty = $(property).find(".property-name").val();

    $.each($(properties).find(".validation-depends-on"), function(index, validationDependsOn) {
      var validationDependsWhen = $(validationDependsOn).parent().parent().find(".validation-depends-when");

      clearSelect(validationDependsWhen);

      var nameDependsOn = $(validationDependsOn).val();

      if (nameDependsOn !== nameProperty || 
          nameDependsOn === "" ||
          primitive === "") {
        return;
      }

      var dependsWhen = getDependsWhen(primitive);

      $.each(dependsWhen, function(index, value) {
        $(validationDependsWhen).append(`<option value="${value}">${value}</option>`);
      });
    });
  });
}

function watcherValidationDependsOnChange() {
  $(document).on("change", ".validation-depends-on", function(validationDependsOn) {
    var entities = $(validationDependsOn.target).parent().parent().parent().parent().parent().parent();

    var input = $(entities).find('input[class="property-name"]').filter(function() { 
      return this.value == $(validationDependsOn.target).val()
    });

    var properties = $(input).parent().parent();

    var row = $(validationDependsOn.target).parent().parent();

    var validationDependsWhen = $(row).find(".validation-depends-when");

    clearSelect(validationDependsWhen);

    var primitive = $(properties).find(".property-primitive").val();
    
    if (!primitive) {
      return;
    }

    var dependsWhen = getDependsWhen(primitive);
    
    $.each(dependsWhen, function(index, value) {
      $(validationDependsWhen).append(`<option value="${value}">${value}</option>`);
    });
  });
}
