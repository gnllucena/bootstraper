<script src="https://code.jquery.com/jquery-3.2.1.min.js"></script>
<script src="html/shared.js"></script>
<script src="html/variables.js"></script>
<script src="html/watchers.js"></script>
<script src="html/manipulations.js"></script>
<link rel="stylesheet" href="html/grid.css">
<link rel="stylesheet" href="html/index.css">
<link rel="stylesheet" href="html/icons.css">

<div class="bootstrap-wrapper">
  <div class="row" style="margin-bottom: -10px;">
    <div class="col-md-6">
      <div class="content-wrapper">
        <div class="content-header-wrapper">
          <label class="content-label">Project</label>
        </div>  
        <div class="row">
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
      </div>
    </div>
    <div class="col-md-6">
      <textarea class="jsonarea"></textarea>
    </div>
  </div>
</div>