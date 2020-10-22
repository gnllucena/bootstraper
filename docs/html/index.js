$(document).ready(function() {
  $(".content-wrapper").after(addEntity());

  clearSelect($(".project-database"));

  $.each(PROJECT_DATABASE, function(index, value) {
    $(".project-database").append($('<option></option>').val(value).html(value));
  });

  $(document).on("blur", ".property-name", function(element) {
    var entity = $(element.target).parent().parent().parent().parent().parent();

    var values = $.map($(entity).find(".property-name"), function(element) {
      var value = $(element).val();

      if (value && value.length > 0) {
        return value;
      }
    });

    $.each($(entity).find(".validation-depends-on"), function(index, element) {
      clearSelect(element);

      $.each(values, function(index, value) {
        $(element).append(`<option value="${value}">${value}</option>`);
      });
    });
  });
});

function clearSelect(element) {
  $(element).empty();
  $(element).append($('<option></option>'));
}

// ======================================================================

function addedNewEntity(element) {
  $(element).parent().parent().append(addEntity(element));
}

function addedNewProperty(element) {
  var properties = $(element).parent().parent().find(".content-entity-properties");

  properties.append(addProperty(element));
}

function addedNewValidation(element) {
  var validations = $(element).parent().parent().find(".content-property-validations");

  validations.append(addValidation(element));
}

function removedExistingEntity(element) {
  $(element).parent().parent().remove();
}

function removedExistingProperty(element) {
  $(element).parent().parent().remove();
}

function removedExistingValidation(element) {
  $(element).parent().parent().remove();
}

// ======================================================================

function addEntity(element) {
  var entity = `
    <div class="content-wrapper entity-wrapper">
      <div class="content-header-wrapper">
        <label class="content-label">Entity</label>
      </div>  
      <div class="content-inputs-wrapper col-10">
        <div class="content-input-wrapper col-2">
          <label>Name</label>
          <input name="entity-name" class="entity-name" />    
        </div>
        
        <div class="content-input-wrapper col-2">
          <label>Table</label>
          <input name="entity-name" class="entity-name" />    
        </div>

        <div class="icon-space" onclick="addedNewProperty(this);">
          <i class="icon gg-math-plus"></i>
        </div>
        
        <div class="icon-space" onclick="removedExistingEntity(this);">
          <i class="icon gg-trash"></i>
        </div>
      </div>

      <div class="content-entity-properties"></div>
    </div>
  `;

  var element = $(entity);

  var properties = $(element).find(".content-entity-properties");

  properties.append(addProperty(element));

  return element;
}

function addProperty(element) {
  var property = `
    <div class="content-wrapper property-wrapper">
      <div class="content-header-wrapper">
        <label class="content-label">Property</label>
      </div>

      <div class="content-inputs-wrapper col-10">
        <div class="content-input-wrapper col-2">
          <label>Name</label>
          <input name="property-name" class="property-name" />    
        </div>

        <div class="content-input-wrapper col-2">
          <label>Column</label>
          <input name="property-column" class="property-column" />
        </div>
      
        <div class="content-input-wrapper col-2">
          <label>Primitive</label>
          <select name="property-primitive" class="property-primitive">
            <option></option>
            replace_property_primitive_options
          </select>
        </div>
        
        <div class="content-input-wrapper col-2">
          <label>Is PK</label>
          <select name="property-primarykey" class="property-primarykey">
            <option></option>
            <option value="true">Yes</option>
            <option value="no">No</option>
          </select>
        </div>
        
        <div class="content-input-wrapper col-2">
          <label>Is Nullable</label>
          <select name="property-nullable" class="property-nullable">
            <option></option>
            <option value="true">Yes</option>
            <option value="no">No</option>
          </select>
        </div>

        <div class="content-input-wrapper col-2">
          <label>Is Unique</label>
          <select name="property-unique" class="property-unique">
            <option></option>
            <option value="true">Yes</option>
            <option value="no">No</option>
          </select>
        </div>

        <div class="icon-space" onclick="addedNewValidation(this);">
          <i class="icon gg-math-plus"></i>
        </div>
        
        <div class="icon-space" onclick="removedExistingProperty(this);">
          <i class="icon gg-trash"></i>
        </div>
      </div>

      <div class="content-property-validations"></div>
    </div>
  `;

  var propertyPrimitiveOptions = $.map(PROPERTY_PRIMITIVES, function(value, index) {
    return `<option value="${value}">${value}</option>`;
  });

  property = property.replace("replace_property_primitive_options", propertyPrimitiveOptions);

  var element = $(property);

  var properties = $(element).find(".content-property-validations");

  properties.append(addValidation(element));

  return element;
}

function addValidation(element) {
  var validation = `
    <div class="content-wrapper validation-wrapper">
      <div class="content-header-wrapper">
        <label class="content-label">Validation</label>
      </div>

      <div class="content-inputs-wrapper col-10">
        <div class="content-input-wrapper col-3">
          <label>Type</label>
          <select name="validation-type" class="validation-type">
            <option></option>
            replace_validation_type_options
          </select>
        </div>
        
        <div class="content-input-wrapper col-2">
          <label>Value</label>
          <input name="validation-value" />    
        </div>

        <div class="content-input-wrapper col-3">
          <label>Depends On</label>
          <select name="validation-depends-on" class="validation-depends-on">
            <option></option>
          </select>
        </div>

        <div class="content-input-wrapper col-3">
          <label>Depends When</label>
          <select name="validation-depends-when" class="validation-depends-when">
            <option></option>
            replace_depends_when_options
          </select>
        </div>

        <div class="icon-space" onclick="removedExistingValidation(this);">
          <i class="icon gg-trash"></i>
        </div>
      </div>
    </div>
  `;

  // var entity = $(element.target).parent().parent().parent().parent().parent();

  // var values = $.map($(entity).find(".property-name"), function(element) {
  //   var value = $(element).val();

  //   if (value && value.length > 0) {
  //     return value;
  //   }
  // });

  // $.each($(entity).find(".validation-depends-on"), function(index, element) {
  //   clearSelect(element);

  //   $.each(values, function(index, value) {
  //     $(element).append(`<option value="${value}">${value}</option>`);
  //   });
  // });

  var dependsWhenOptions = $.map(DEPENDS_WHEN, function(value, index) {
    return `<option value="${value}">${value}</option>`;
  });

  var validationTypeOptions = $.map(VALIDATION_TYPES, function(value, index) {
    return `<option value="${value}">${value}</option>`;
  });

  validation = validation.replace("replace_depends_when_options", dependsWhenOptions);
  validation = validation.replace("replace_validation_type_options", validationTypeOptions);

  return $(validation);
}