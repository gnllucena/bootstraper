$(document).ready(function() {
  $(".content").append(addProject());
});

function addedNewEntity(element) {
  $(element).parent().parent().append(addEntity());
}

function addedNewProperty(element) {
  var properties = $(element).parent().parent().find(".content-entity-properties");

  var entityWrapper = $(element).parent().parent().parent();

  properties.append(addProperty(entityWrapper));
}

function addedNewValidation(element) {
  var validations = $(element).parent().parent().find(".content-property-validations");

  var entityWrapper = $(element).parent().parent().parent().parent().parent();

  var propertyWrapper = $(element).parent();

  validations.append(addValidation(entityWrapper, propertyWrapper));
}

function removedExistingEntity(element) {
  $(element).parent().parent().remove();
}

function removedExistingValidation(element) {
  $(element).parent().parent().remove();
}

function addProject(element) {
  var row = `
    <div class="content-wrapper">
      <div class="content-header-wrapper">
        <label class="content-label">Project</label>
      </div>  
      <div class="row" style="margin-bottom: -10px;">
        <div class="content-input-wrapper col-md-4">
          <label>Name</label>
          <input name="project-name" class="project-name" />
        </div>
        
        <div class="content-input-wrapper col-md-4">
          <label>Database</label>
          <select name="project-database" class="project-database"></select>
        </div>

        <div class="icon-space" onclick="addedNewEntity(this);">
          <i class="icon gg-math-plus"></i>
        </div>
      </div>

      <div class="content-project-entities"></div>
    </div>
  `;

  var project = $(row);

  var entities = $(project).find(".content-project-entities");

  entities.append(addEntity(element));

  return project;
}

function addEntity(element) {
  var row = `
    <div class="content-wrapper entity-wrapper">
      <div class="content-header-wrapper">
        <label class="content-label">Entity</label>
      </div>  
      <div class="row">
        <div class="content-input-wrapper col-md-4">
          <label>Name</label>
          <input name="entity-name" class="entity-name" />    
        </div>
        
        <div class="content-input-wrapper col-md-4">
          <label>Table</label>
          <input name="entity-name" class="entity-name" />    
        </div>

        <div class="icon-space add-property" onclick="addedNewProperty(this);">
          <i class="icon gg-math-plus"></i>
        </div>
        
        <div class="icon-space remove-entity" onclick="removedExistingEntity(this);">
          <i class="icon gg-trash"></i>
        </div>
      </div>

      <div class="content-entity-properties"></div>
    </div>
  `;

  var entity = $(row);

  var properties = $(entity).find(".content-entity-properties");

  properties.append(addProperty(element));

  return entity;
}

function addProperty(entityWrapper) {
  var row = `
    <div class="content-wrapper property-wrapper">
      <div class="content-header-wrapper">
        <label class="content-label">Property</label>
      </div>

      <div class="row">
        <div class="content-input-wrapper col-md-3">
          <label>Name</label>
          <input name="property-name" class="property-name" />    
        </div>

        <div class="content-input-wrapper col-md-3">
          <label>Column</label>
          <input name="property-column" class="property-column" />
        </div>
      
        <div class="content-input-wrapper col-md-3">
          <label>Primitive</label>
          <select name="property-primitive" class="property-primitive">
            <option></option>
            replace_property_primitive_options
          </select>
        </div>

        <div class="icon-space add-validation" onclick="addedNewValidation(this);">
          <i class="icon gg-math-plus"></i>
        </div>
        
        <div class="icon-space remove-property">
          <i class="icon gg-trash"></i>
        </div>
      </div>
      <div class="row">
        <div class="content-input-wrapper col-md-3">
          <label>Is PK</label>
          <select name="property-primarykey" class="property-primarykey">
            <option></option>
            <option value="true">Yes</option>
            <option value="no">No</option>
          </select>
        </div>
        
        <div class="content-input-wrapper col-md-3">
          <label>Is Null</label>
          <select name="property-nullable" class="property-nullable">
            <option></option>
            <option value="true">Yes</option>
            <option value="no">No</option>
          </select>
        </div>

        <div class="content-input-wrapper col-md-3">
          <label>Is Unique</label>
          <select name="property-unique" class="property-unique">
            <option></option>
            <option value="true">Yes</option>
            <option value="no">No</option>
          </select>
        </div>
      </div>

      <div class="content-property-validations"></div>
    </div>
  `;

  var propertyPrimitiveOptions = $.map(PROPERTY_PRIMITIVES, function(value, index) {
    return `<option value="${value}">${value}</option>`;
  });

  row = row.replace("replace_property_primitive_options", propertyPrimitiveOptions);

  var property = $(row);

  var validations = $(property).find(".content-property-validations");

  validations.append(addValidation(entityWrapper));

  return property;
}

function addValidation(entityWrapper, propertyWrapper) {
  var row = `
    <div class="content-wrapper validation-wrapper">
      <div class="content-header-wrapper">
        <label class="content-label">Validation</label>
      </div>

      <div class="row">
        <div class="content-input-wrapper col-md-4">
          <label>Type</label>
          <select name="validation-type" class="validation-type">
            <option></option>
            replace_validation_type_options
          </select>
        </div>
        
        <div class="content-input-wrapper col-md-4">
          <label>Value</label>
          <input name="validation-value" />    
        </div>
        
        <div class="icon-space remove-validation" onclick="removedExistingValidation(this);">
          <i class="icon gg-trash"></i>
        </div>
      </div>
      <div class="row">
        <div class="content-input-wrapper col-md-4">
          <label>Depends On</label>
          <select name="validation-depends-on" class="validation-depends-on">
            <option></option>
            replace_depends_on_options
          </select>
        </div>

        <div class="content-input-wrapper col-md-4">
          <label>Depends When</label>
          <select name="validation-depends-when" class="validation-depends-when">
            <option></option>
            replace_depends_when_options
          </select>
        </div>
      </div>
    </div>
  `;

  if (entityWrapper) {
    var values = getPropertyNameValues(entityWrapper);

    var dependsOnOptions = $.map(values, function(value, index) {
      return `<option value="${value}">${value}</option>`;
    });

    row = row.replace("replace_depends_on_options", dependsOnOptions);
  }

  if (propertyWrapper) {
    var primitive = $(propertyWrapper).find(".property-primitive").val();

    if (primitive) {
      var validations = getValidations(primitive);

      var validationTypesOptions = $.map(validations, function(value, index) {
        return `<option value="${value}">${value}</option>`;
      });

      row = row.replace("replace_validation_type_options", validationTypesOptions);
    }
  }

  return $(row);
}

function getPropertyNameValues(entity) {
  var values = $.map($(entity).find(".property-name"), function(element) {
    var value = $(element).val();

    if (value && value.length > 0) {
      return value;
    }
  });

  return values;
}  
