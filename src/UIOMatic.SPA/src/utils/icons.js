// Map UIOMatic icons to Font Awesome icons
export const iconMap = {
  // Basic icons
  'icon-users': 'fa-users',
  'icon-user': 'fa-user',
  'icon-folder': 'fa-folder',
  'icon-document': 'fa-file-alt',
  'icon-movie': 'fa-film',
  'icon-image': 'fa-image',
  'icon-picture': 'fa-image',
  'icon-photo': 'fa-image',
  'icon-file': 'fa-file',
  'icon-text': 'fa-file-alt',
  'icon-article': 'fa-newspaper',
  'icon-post': 'fa-newspaper',
  'icon-page': 'fa-file-alt',
  
  // Navigation icons
  'icon-menu': 'fa-bars',
  'icon-link': 'fa-link',
  'icon-url': 'fa-link',
  'icon-search': 'fa-search',
  'icon-sort': 'fa-sort',
  'icon-sort-asc': 'fa-sort-up',
  'icon-sort-desc': 'fa-sort-down',
  
  // Media icons
  'icon-video': 'fa-video',
  'icon-audio': 'fa-music',
  'icon-podcast': 'fa-podcast',
  'icon-gallery': 'fa-images',
  'icon-slider': 'fa-images',
  
  // Communication icons
  'icon-email': 'fa-envelope',
  'icon-phone': 'fa-phone',
  'icon-message': 'fa-comment',
  'icon-comment': 'fa-comment',
  'icon-contact': 'fa-address-card',
  'icon-social': 'fa-share-alt',
  'icon-share': 'fa-share-alt',
  
  // Location icons
  'icon-location': 'fa-map-marker-alt',
  'icon-map': 'fa-map',
  'icon-address': 'fa-map-marker-alt',
  
  // Calendar icons
  'icon-event': 'fa-calendar',
  'icon-calendar': 'fa-calendar',
  
  // Action icons
  'icon-edit': 'fa-edit',
  'icon-delete': 'fa-trash',
  'icon-create': 'fa-plus',
  
  // Status icons
  'icon-success': 'fa-check-circle',
  'icon-error': 'fa-times-circle',
  'icon-warning': 'fa-exclamation-triangle',
  'icon-info': 'fa-info-circle',
  'icon-help': 'fa-question-circle',
  
  // Settings icons
  'icon-settings': 'fa-cog',
  'icon-config': 'fa-cog',
  'icon-options': 'fa-cog',
  'icon-tools': 'fa-tools',
  
  // Dashboard icons
  'icon-dashboard': 'fa-tachometer-alt',
  'icon-stats': 'fa-chart-bar',
  'icon-analytics': 'fa-chart-line',
  'icon-report': 'fa-chart-pie',
  
  // User management icons
  'icon-login': 'fa-sign-in-alt',
  'icon-logout': 'fa-sign-out-alt',
  'icon-register': 'fa-user-plus',
  'icon-signup': 'fa-user-plus',
  'icon-password': 'fa-key',
  'icon-security': 'fa-shield-alt',
  'icon-privacy': 'fa-user-shield',
  'icon-permission': 'fa-lock',
  'icon-role': 'fa-user-tag',
  'icon-access': 'fa-unlock',
  'icon-admin': 'fa-user-cog',
  'icon-superuser': 'fa-user-shield',
  'icon-moderator': 'fa-user-tie',
  'icon-editor': 'fa-user-edit',
  'icon-author': 'fa-user-edit',
  'icon-contributor': 'fa-user-edit',
  
  // Content management icons
  'icon-form': 'fa-wpforms',
  'icon-category': 'fa-tags',
  'icon-tag': 'fa-tag',
  'icon-group': 'fa-users',
  'icon-team': 'fa-users',
  'icon-member': 'fa-user',
  'icon-profile': 'fa-user',
  'icon-account': 'fa-user',
  
  // E-commerce icons
  'icon-product': 'fa-shopping-cart',
  'icon-cart': 'fa-shopping-cart',
  'icon-basket': 'fa-shopping-basket',
  'icon-bag': 'fa-shopping-bag',
  'icon-order': 'fa-shopping-cart',
  'icon-purchase': 'fa-shopping-cart',
  'icon-sale': 'fa-cash-register',
  'icon-payment': 'fa-credit-card',
  'icon-invoice': 'fa-file-invoice-dollar',
  'icon-receipt': 'fa-receipt',
  
  // Notification icons
  'icon-notification': 'fa-bell',
  'icon-alert': 'fa-exclamation-circle',
  'icon-activity': 'fa-history',
  'icon-history': 'fa-history',
  'icon-log': 'fa-clipboard-list',
  
  // Documentation icons
  'icon-docs': 'fa-book',
  'icon-guide': 'fa-book',
  'icon-tutorial': 'fa-graduation-cap',
  'icon-learn': 'fa-graduation-cap',
  'icon-education': 'fa-graduation-cap',
  'icon-course': 'fa-graduation-cap',
  'icon-lesson': 'fa-book-reader',
  'icon-module': 'fa-book',
  'icon-chapter': 'fa-bookmark',
  'icon-section': 'fa-bookmark',
  'icon-topic': 'fa-bookmark',
  'icon-subject': 'fa-bookmark',
  'icon-faq': 'fa-question-circle'
};

// Get the appropriate icon class
export const getIconClass = (iconName) => {
  if (!iconName) return 'fa-folder';
  return iconMap[iconName] || 'fa-folder';
}; 