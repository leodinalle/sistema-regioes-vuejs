<template>
  <div class="container">
    <div class="main-content">
      <!-- Header -->
      <div class="header">
        <h1 class="title">Sistema de Regiões</h1>
        <p class="subtitle">Gerencie regiões por estado de forma simples e eficiente</p>
      </div>

      <!-- Formulário de Cadastro -->
      <div class="form-card">
        <div class="form-header">
          <MapPinIcon class="icon" />
          <h2 class="form-title">
            {{ editingRegion ? 'Editar Região' : 'Cadastro de Regiões' }}
          </h2>
        </div>

        <form @submit.prevent="submitForm" class="form">
          <div class="form-grid">
            <!-- Campo UF -->
            <div class="form-group">
              <label class="label">
                UF <span class="required">*</span>
              </label>
              <select 
                v-model="form.uf" 
                class="select"
                :class="{ 'error': errors.uf }"
                required
              >
                <option value="">Selecione</option>
                <option v-for="uf in ufs" :key="uf" :value="uf">{{ uf }}</option>
              </select>
              <p v-if="errors.uf" class="error-message">{{ errors.uf }}</p>
            </div>

            <!-- Campo Região -->
            <div class="form-group">
              <label class="label">
                Região <span class="required">*</span>
              </label>
              <input 
                v-model="form.nome" 
                type="text" 
                class="input"
                :class="{ 'error': errors.nome }"
                placeholder="Digite o nome da região"
                required
              />
              <p v-if="errors.nome" class="error-message">{{ errors.nome }}</p>
            </div>
          </div>

          <!-- Botões -->
          <div class="button-group">
            <button 
              type="submit" 
              :disabled="loading"
              class="btn btn-primary"
            >
              <SaveIcon class="btn-icon" />
              {{ loading ? 'Processando...' : (editingRegion ? 'Atualizar' : 'Inserir') }}
            </button>
            
            <button 
              v-if="editingRegion"
              type="button" 
              @click="cancelEdit"
              class="btn btn-secondary"
            >
              <XIcon class="btn-icon" />
              Cancelar
            </button>
          </div>
        </form>
      </div>

      <!-- Mensagens de Feedback -->
      <div v-if="message" class="message-container">
        <div :class="['message', message.type === 'success' ? 'message-success' : 'message-error']">
          <CheckCircleIcon v-if="message.type === 'success'" class="message-icon" />
          <AlertCircleIcon v-else class="message-icon" />
          {{ message.text }}
        </div>
      </div>

      <!-- Tabela de Regiões -->
      <div class="table-card">
        <div class="table-header">
          <div class="table-title-section">
            <ListIcon class="icon" />
            <h3 class="table-title">Regiões Cadastradas</h3>
          </div>
          <div class="table-count">
            Total: {{ regions.length }} região(ões)
          </div>
        </div>

        <div v-if="loadingRegions" class="loading">
          <div class="loading-content">
            <div class="spinner"></div>
            Carregando regiões...
          </div>
        </div>

        <div v-else-if="regions.length === 0" class="empty-state">
          <MapIcon class="empty-icon" />
          <p class="empty-title">Nenhuma região cadastrada</p>
          <p class="empty-subtitle">Adicione a primeira região usando o formulário acima</p>
        </div>

        <div v-else class="table-container">
          <table class="table">
            <thead class="table-head">
              <tr>
                <th class="th">UF</th>
                <th class="th">Região</th>
                <th class="th th-center">Situação</th>
                <th class="th th-center">Ações</th>
              </tr>
            </thead>
            <tbody class="table-body">
              <tr v-for="region in sortedRegions" :key="region.id" class="tr">
                <td class="td td-bold">{{ region.uf }}</td>
                <td class="td">{{ region.nome }}</td>
                <td class="td td-center">
                  <span :class="['status', region.ativo ? 'status-active' : 'status-inactive']">
                    {{ region.ativo ? 'Ativo' : 'Inativo' }}
                  </span>
                </td>
                <td class="td td-center">
                  <div class="actions">
                    <button 
                      @click="editRegion(region)"
                      class="action-btn action-edit"
                    >
                      Editar
                    </button>
                    <button 
                      v-if="region.ativo"
                      @click="confirmDeactivate(region)"
                      class="action-btn action-deactivate"
                    >
                      Inativar
                    </button>
                    <button 
                      v-else
                      @click="activateRegion(region)"
                      class="action-btn action-activate"
                    >
                      Ativar
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Modal de Confirmação -->
      <div v-if="showConfirmModal" class="modal-overlay">
        <div class="modal">
          <div class="modal-header">
            <AlertTriangleIcon class="modal-icon" />
            <h3 class="modal-title">Confirmar Inativação</h3>
          </div>
          <p class="modal-text">
            Tem certeza que deseja inativar a região <strong>{{ regionToDeactivate?.nome }}</strong> 
            do estado <strong>{{ regionToDeactivate?.uf }}</strong>?
          </p>
          <div class="modal-actions">
            <button 
              @click="cancelDeactivate"
              class="btn btn-outline"
            >
              Cancelar
            </button>
            <button 
              @click="deactivateRegion"
              class="btn btn-danger"
            >
              Inativar
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { 
  MapPinIcon, 
  SaveIcon, 
  XIcon, 
  CheckCircleIcon, 
  AlertCircleIcon, 
  ListIcon, 
  MapIcon,
  AlertTriangleIcon
} from 'lucide-vue-next'

// Estados reativos
const regions = ref([])
const form = ref({ uf: '', nome: '' })
const errors = ref({})
const message = ref(null)
const loading = ref(false)
const loadingRegions = ref(false)
const editingRegion = ref(null)
const showConfirmModal = ref(false)
const regionToDeactivate = ref(null)

// Lista de UFs brasileiras
const ufs = ref([
  'AC', 'AL', 'AP', 'AM', 'BA', 'CE', 'DF', 'ES', 'GO', 'MA',
  'MT', 'MS', 'MG', 'PA', 'PB', 'PR', 'PE', 'PI', 'RJ', 'RN',
  'RS', 'RO', 'RR', 'SC', 'SP', 'SE', 'TO'
])

// URL base da API
const API_BASE_URL = '/api'

// Computed para ordenação
const sortedRegions = computed(() => {
  return [...regions.value].sort((a, b) => {
    if (a.uf !== b.uf) {
      return a.uf.localeCompare(b.uf)
    }
    return a.nome.localeCompare(b.nome)
  })
})

// Funções da API - CORRIGIDA para tratar erro 409
const apiCall = async (endpoint, options = {}) => {
  try {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      headers: {
        'Content-Type': 'application/json',
        ...options.headers
      },
      ...options
    })
        
    if (!response.ok) {
      // Tratamento específico para erro 409 (região já existe)
      if (response.status === 409) {
        const errorMessage = await response.text()
        throw new Error(errorMessage)
      }
      
      // Para outros erros
      throw new Error(`Erro ${response.status}`)
    }
        
    // Se não há conteúdo (status 204), retornar null
    if (response.status === 204 || response.headers.get('content-length') === '0') {
      return null
    }
        
    // Verificar se há conteúdo para parsear
    const contentType = response.headers.get('content-type')
    if (contentType && contentType.includes('application/json')) {
      return await response.json()
    }
        
    return null
  } catch (error) {
    console.error('API Error:', error)
    throw error
  }
}

// Carregar regiões
const loadRegions = async () => {
  loadingRegions.value = true
  try {
    const data = await apiCall('/region')
    regions.value = data || []
  } catch (error) {
    showMessage('Erro ao carregar regiões: ' + error.message, 'error')
  } finally {
    loadingRegions.value = false
  }
}

// Submeter formulário
const submitForm = async () => {
  clearErrors()
  
  if (!validateForm()) return
  
  loading.value = true
  
  try {
    if (editingRegion.value) {
      await apiCall('/region', {
        method: 'PUT', 
        body: JSON.stringify({ 
          ...form.value, 
          id: editingRegion.value.id 
        })
      })
      showMessage('Região atualizada com sucesso!', 'success')
    } else {
      await apiCall('/region', {
        method: 'POST',
        body: JSON.stringify({ ...form.value, ativo: true })
      })
      showMessage('Região cadastrada com sucesso!', 'success')
    }
    
    resetForm()
    await loadRegions()
  } catch (error) {
    showMessage('Erro ao salvar região: ' + error.message, 'error')
  } finally {
    loading.value = false
  }
}

// Validar formulário
const validateForm = () => {
  let isValid = true
  
  if (!form.value.uf) {
    errors.value.uf = 'UF é obrigatória'
    isValid = false
  }
  
  if (!form.value.nome?.trim()) {
    errors.value.nome = 'Região é obrigatória'
    isValid = false
  }
  
  return isValid
}

// Editar região
const editRegion = (region) => {
  editingRegion.value = region
  form.value = { uf: region.uf, nome: region.nome }
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

// Cancelar edição
const cancelEdit = () => {
  editingRegion.value = null
  resetForm()
}

// Confirmar inativação
const confirmDeactivate = (region) => {
  regionToDeactivate.value = region
  showConfirmModal.value = true
}

// Cancelar inativação
const cancelDeactivate = () => {
  regionToDeactivate.value = null
  showConfirmModal.value = false
}

// Inativar região
const deactivateRegion = async () => {
  try {
    await apiCall(`/region/${regionToDeactivate.value.id}/inactive`, {
      method: 'PATCH'
    })
    showMessage('Região inativada com sucesso!', 'success')
    await loadRegions()
  } catch (error) {
    showMessage('Erro ao inativar região: ' + error.message, 'error')
  } finally {
    showConfirmModal.value = false
    regionToDeactivate.value = null
  }
}

// Ativar região
const activateRegion = async (region) => {
  try {
    await apiCall(`/region/${region.id}/active`, {
      method: 'PATCH'
    })
    showMessage('Região ativada com sucesso!', 'success')
    await loadRegions()
  } catch (error) {
    showMessage('Erro ao ativar região: ' + error.message, 'error')
  }
}

// Funções auxiliares
const resetForm = () => {
  form.value = { uf: '', nome: '' }
  editingRegion.value = null
  clearErrors()
}

const clearErrors = () => {
  errors.value = {}
}

const showMessage = (text, type) => {
  message.value = { text, type }
  setTimeout(() => {
    message.value = null
  }, 5000)
}

// Carregar dados ao montar o componente
onMounted(() => {
  loadRegions()
})
</script>

<style scoped>
/* Reset e base */
* {
  box-sizing: border-box;
}

.container {
  min-height: 100vh;
  background: linear-gradient(135deg, #e3f2fd 0%, #e8eaf6 100%);
  padding: 24px;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

.main-content {
  max-width: 1200px;
  margin: 0 auto;
}

/* Header */
.header {
  text-align: center;
  margin-bottom: 32px;
}

.title {
  font-size: 2.5rem;
  font-weight: bold;
  color: #1a202c;
  margin: 0 0 8px 0;
}

.subtitle {
  color: #718096;
  margin: 0;
  font-size: 1.1rem;
}

/* Cards */
.form-card, .table-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  margin-bottom: 32px;
}

.form-card {
  padding: 24px;
}

/* Form Header */
.form-header {
  display: flex;
  align-items: center;
  margin-bottom: 24px;
}

.icon {
  width: 24px;
  height: 24px;
  color: #4f46e5;
  margin-right: 8px;
}

.form-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #1a202c;
  margin: 0;
}

/* Form */
.form {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

@media (max-width: 768px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}

.form-group {
  display: flex;
  flex-direction: column;
}

.label {
  font-size: 0.875rem;
  font-weight: 500;
  color: #374151;
  margin-bottom: 8px;
}

.required {
  color: #ef4444;
}

.input, .select {
  padding: 12px 16px;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 1rem;
  transition: all 0.2s;
}

.input:focus, .select:focus {
  outline: none;
  border-color: #4f46e5;
  box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.1);
}

.input.error, .select.error {
  border-color: #ef4444;
}

.error-message {
  color: #ef4444;
  font-size: 0.875rem;
  margin-top: 4px;
  margin-bottom: 0;
}

/* Buttons */
.button-group {
  display: flex;
  gap: 12px;
  padding-top: 16px;
}

.btn {
  display: flex;
  align-items: center;
  padding: 12px 24px;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  text-decoration: none;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-icon {
  width: 16px;
  height: 16px;
  margin-right: 8px;
}

.btn-primary {
  background: #4f46e5;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: #4338ca;
}

.btn-secondary {
  background: #6b7280;
  color: white;
}

.btn-secondary:hover {
  background: #4b5563;
}

.btn-outline {
  background: transparent;
  color: #6b7280;
  border: 1px solid #d1d5db;
}

.btn-outline:hover {
  background: #f9fafb;
}

.btn-danger {
  background: #ef4444;
  color: white;
}

.btn-danger:hover {
  background: #dc2626;
}

/* Messages */
.message-container {
  margin-bottom: 24px;
}

.message {
  display: flex;
  align-items: center;
  padding: 16px;
  border-radius: 8px;
  border: 1px solid;
}

.message-success {
  background: #f0fdf4;
  color: #166534;
  border-color: #bbf7d0;
}

.message-error {
  background: #fef2f2;
  color: #991b1b;
  border-color: #fecaca;
}

.message-icon {
  width: 20px;
  height: 20px;
  margin-right: 8px;
}

/* Table */
.table-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 24px;
  border-bottom: 1px solid #e5e7eb;
}

.table-title-section {
  display: flex;
  align-items: center;
}

.table-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #1a202c;
  margin: 0;
}

.table-count {
  font-size: 0.875rem;
  color: #6b7280;
}

/* Loading */
.loading {
  padding: 64px;
  text-align: center;
}

.loading-content {
  display: inline-flex;
  align-items: center;
  gap: 8px;
}

.spinner {
  width: 24px;
  height: 24px;
  border: 2px solid #e5e7eb;
  border-top: 2px solid #4f46e5;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

/* Empty State */
.empty-state {
  padding: 64px;
  text-align: center;
  color: #6b7280;
}

.empty-icon {
  width: 48px;
  height: 48px;
  margin: 0 auto 16px;
  color: #9ca3af;
}

.empty-title {
  font-size: 1.125rem;
  font-weight: 500;
  margin: 0 0 4px 0;
}

.empty-subtitle {
  font-size: 0.875rem;
  margin: 0;
}

/* Table */
.table-container {
  overflow-x: auto;
}

.table {
  width: 100%;
  border-collapse: collapse;
}

.table-head {
  background: #f9fafb;
}

.th {
  padding: 16px 24px;
  text-align: left;
  font-size: 0.875rem;
  font-weight: 600;
  color: #374151;
}

.th-center {
  text-align: center;
}

.table-body .tr:hover {
  background: #f9fafb;
}

.td {
  padding: 16px 24px;
  font-size: 0.875rem;
  color: #374151;
  border-top: 1px solid #e5e7eb;
}

.td-bold {
  font-weight: 500;
  color: #1a202c;
}

.td-center {
  text-align: center;
}

/* Status */
.status {
  display: inline-flex;
  padding: 4px 12px;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 500;
}

.status-active {
  background: #f0fdf4;
  color: #166534;
}

.status-inactive {
  background: #fef2f2;
  color: #991b1b;
}

/* Actions */
.actions {
  display: flex;
  justify-content: center;
  gap: 8px;
}

.action-btn {
  background: none;
  border: none;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 4px;
  transition: all 0.2s;
}

.action-edit {
  color: #4f46e5;
}

.action-edit:hover {
  color: #4338ca;
  background: #eef2ff;
}

.action-deactivate {
  color: #ef4444;
}

.action-deactivate:hover {
  color: #dc2626;
  background: #fef2f2;
}

.action-activate {
  color: #059669;
}

.action-activate:hover {
  color: #047857;
  background: #f0fdf4;
}

/* Modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
}

.modal {
  background: white;
  border-radius: 8px;
  padding: 24px;
  max-width: 400px;
  width: 100%;
  margin: 16px;
}

.modal-header {
  display: flex;
  align-items: center;
  margin-bottom: 16px;
}

.modal-icon {
  width: 24px;
  height: 24px;
  color: #ef4444;
  margin-right: 8px;
}

.modal-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1a202c;
  margin: 0;
}

.modal-text {
  color: #6b7280;
  margin-bottom: 24px;
  line-height: 1.5;
}

.modal-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
}
</style>
