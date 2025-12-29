/**
 * UUID Utility for generating and handling GUIDs compatible with C# Guid
 */

/**
 * Empty GUID constant - used when no value is provided
 * Tương đương với Guid.Empty trong C#
 */
export const EMPTY_GUID = '00000000-0000-0000-0000-000000000000'

/**
 * Generate a new UUID v4 (compatible with C# Guid)
 * @returns {string} UUID string in format: xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx
 */
export function generateUUID() {
  // Use crypto.randomUUID if available (modern browsers)
  if (typeof crypto !== 'undefined' && crypto.randomUUID) {
    return crypto.randomUUID()
  }
  
  // Fallback for older browsers
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    const r = Math.random() * 16 | 0
    const v = c === 'x' ? r : (r & 0x3 | 0x8)
    return v.toString(16)
  })
}

/**
 * Check if a GUID is empty or null
 * @param {string} guid 
 * @returns {boolean}
 */
export function isEmptyGuid(guid) {
  return !guid || guid === EMPTY_GUID
}

/**
 * Get GUID or EMPTY_GUID if value is falsy
 * @param {string} guid 
 * @returns {string}
 */
export function getGuidOrEmpty(guid) {
  return guid || EMPTY_GUID
}

export default {
  EMPTY_GUID,
  generateUUID,
  isEmptyGuid,
  getGuidOrEmpty
}
