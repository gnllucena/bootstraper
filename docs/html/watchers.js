$(document).ready(function() {
  watcherPropertyNameChange();
  watcherPropertyPrimitiveChange();
  watcherValidationDependsOnChange();
});

function watcherPropertyNameChange() {
  $(document).on("blur", ".property-name", function(element) {
    var entity = $(element.target).parent().parent().parent().parent().parent();

    var values = getPropertyNameValues(entity);

    $.each($(entity).find(".validation-depends-on"), function(index, element) {
      clearSelect(element);

      $.each(values, function(index, value) {
        $(element).append(`<option value="${value}">${value}</option>`);
      });
    });
  });
}

function watcherPropertyPrimitiveChange() {
  $(document).on("change", ".property-primitive", function(primitiveElement) {
    var property = $(primitiveElement.target).parent().parent().parent();

    var primitive = $(property).find('select[class="property-primitive"]').val();

    $.each($(property).find(".validation-type"), function(index, validationTypeElement) {
      clearSelect(validationTypeElement);

      if (!primitive) {
        return;
      }

      var validations = getValidations(primitive);

      $.each(validations, function(index, value) {
        $(validationTypeElement).append(`<option value="${value}">${value}</option>`);
      });
    });

    var properties = $(property).parent();

    $.each($(properties).find(".validation-depends-on"), function(index, validationDependsOnElement) {
      // clearSelect(validationDependsWhenElement);


      if (!primitive) {
        return;
      }

      var dependsWhen = getDependsWhen(primitive);

      $.each(dependsWhen, function(index, value) {
        $(validationDependsWhenElement).append(`<option value="${value}">${value}</option>`);
      });
    });
  });
}

function watcherValidationDependsOnChange() {
  $(document).on("change", ".validation-depends-on", function(validationDependsOnElement) {
    var entities = $(validationDependsOnElement.target).parent().parent().parent().parent().parent().parent();

    var input = $(entities).find('input[class="property-name"]').filter(function() { 
      return this.value == $(validationDependsOnElement.target).val()
    });

    var properties = $(input).parent().parent();

    var row = $(validationDependsOnElement.target).parent().parent();

    var validationDependsWhen = $(row).find(".validation-depends-when");

    clearSelect(validationDependsWhen);

    var primitive = $(properties).find(".property-primitive").val();
    
    if (!primitive) {
      return;
    }

    var dependsWhen = getDependsWhen(primitive);
    
    $.each(dependsWhen, function(index, value) {
      $(input).append(`<option value="${value}">${value}</option>`);
    });
  });
}
