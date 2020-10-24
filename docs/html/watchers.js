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
    var entity = $(primitiveElement.target).parent().parent().parent();

    var primitive = $(entity).find('select[class="property-primitive"]').val();

    $.each($(entity).find(".validation-type"), function(index, validationElement) {
      clearSelect(validationElement);

      var validations = getValidations(primitive);

      $.each(validations, function(index, value) {
        $(validationElement).append(`<option value="${value}">${value}</option>`);
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

    var primitive = $(properties).find(".property-primitive").val();
    
    $.each($(entity).find(".validation-depends-when"), function(index, validationDependsWhenElement) {
      clearSelect(validationDependsWhenElement);

      var dependsWhen = getDependsWhen(primitive);

      $.each(dependsWhen, function(index, value) {
        $(validationDependsWhenElement).append(`<option value="${value}">${value}</option>`);
      });
    });
  });
}
