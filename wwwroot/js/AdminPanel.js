const $ = require("jquery") // Declare the $ variable

$(document).ready(() => {
  let isMobileOpen = false

  // Desktop sidebar hover effect
  $(".sidebar").hover(
    () => {
      // On hover in
      $(".main-content").addClass("expanded")
    },
    () => {
      // On hover out
      $(".main-content").removeClass("expanded")
    },
  )

  // Mobile menu toggle
  $("#mobile-menu-btn").click(function () {
    isMobileOpen = !isMobileOpen
    if (isMobileOpen) {
      $(".sidebar-mobile").addClass("open")
      $("#mobile-overlay").removeClass("hidden")
      $(this).find("i").removeClass("fa-bars").addClass("fa-times")
    } else {
      $(".sidebar-mobile").removeClass("open")
      $("#mobile-overlay").addClass("hidden")
      $(this).find("i").removeClass("fa-times").addClass("fa-bars")
    }
  })

  // Close mobile menu when clicking overlay
  $("#mobile-overlay").click(function () {
    isMobileOpen = false
    $(".sidebar-mobile").removeClass("open")
    $(this).addClass("hidden")
    $("#mobile-menu-btn").find("i").removeClass("fa-times").addClass("fa-bars")
  })

  // Navigation
  $(".nav-item").click(function (e) {
    e.preventDefault()
    const target = $(this).data("target")

    // Update active state
    $(".nav-item").removeClass("active")
    $(this).addClass("active")

    // Close mobile menu
    if (isMobileOpen) {
      $(".sidebar-mobile").removeClass("open")
      $("#mobile-overlay").addClass("hidden")
      $("#mobile-menu-btn").find("i").removeClass("fa-times").addClass("fa-bars")
      isMobileOpen = false
    }

    // Navigate to page
    window.location.href = target
  })

  // Modal functions
  window.openModal = (modalId) => {
    $("#" + modalId)
      .removeClass("hidden")
      .addClass("flex")
    $("#" + modalId + " .modal-content").addClass("animate-modal-slide-in")
    $("body").addClass("overflow-hidden")
  }

  window.closeModal = (modalId) => {
    $("#" + modalId)
      .addClass("hidden")
      .removeClass("flex")
    $("#" + modalId + " .modal-content").removeClass("animate-modal-slide-in")
    $("body").removeClass("overflow-hidden")

    // Clear form
    $("#" + modalId + " form")[0].reset()
    $("#" + modalId + " .error-message").addClass("hidden")
  }

  // Close modal when clicking backdrop
  $(".modal-backdrop").click(function (e) {
    if (e.target === this) {
      const modalId = $(this).attr("id")
      window.closeModal(modalId)
    }
  })

  // Form submission
  window.submitForm = (formId, endpoint, modalId) => {
    const form = $("#" + formId)
    const formData = {}

    // Collect form data
    form.find("input, select, textarea").each(function () {
      const field = $(this)
      const name = field.attr("name")
      const value = field.val()

      if (name && value) {
        formData[name] = value
      }
    })

    // Show loading state
    const submitBtn = form.find('button[type="submit"]')
    const originalText = submitBtn.html()
    submitBtn.html('<i class="fas fa-spinner fa-spin mr-2"></i>Creating...').prop("disabled", true)

    // Clear previous errors
    form.find(".error-message").addClass("hidden")

    // Submit form
    $.ajax({
      url: endpoint,
      type: "POST",
      contentType: "application/json",
      data: JSON.stringify(formData),
      success: (response) => {
        if (response.success) {
          window.closeModal(modalId)
          window.showNotification(response.message, "success")
          // Reload page to show new data
          setTimeout(() => {
            window.location.reload()
          }, 1000)
        } else {
          showErrors(form, response.errors)
        }
      },
      error: () => {
        window.showNotification("An error occurred. Please try again.", "error")
      },
      complete: () => {
        submitBtn.html(originalText).prop("disabled", false)
      },
    })
  }

  // Show errors
  function showErrors(form, errors) {
    if (errors && errors.length > 0) {
      const errorContainer = form.find(".error-message")
      errorContainer
        .html('<ul class="list-disc list-inside">' + errors.map((error) => "<li>" + error + "</li>").join("") + "</ul>")
        .removeClass("hidden")
    }
  }

  // Show notification
  window.showNotification = (message, type) => {
    const bgColor = type === "success" ? "bg-green-500" : "bg-red-500"
    const notification = $(`
            <div class="fixed top-4 right-4 ${bgColor} text-white px-6 py-3 rounded-lg shadow-lg z-50 animate-fade-in">
                <div class="flex items-center">
                    <i class="fas fa-${type === "success" ? "check" : "exclamation-triangle"} mr-2"></i>
                    ${message}
                </div>
            </div>
        `)

    $("body").append(notification)

    setTimeout(() => {
      notification.fadeOut(() => {
        notification.remove()
      })
    }, 3000)
  }
})
