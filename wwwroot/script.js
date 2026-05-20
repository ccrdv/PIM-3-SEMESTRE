// ==================== NAVEGAÇÃO ====================
function clickInicio() {
    window.location.href = "index.html";
}

function clickMenuVenda() {
    window.location.href = "novaVenda.html";
}

function clickMenuProdutos() {
    window.location.href = "produtos.html";
}

function clickMenuFiado() {
    window.location.href = "fiado.html";
}

function clickMenuEstoque() {
    window.location.href = "estoque.html";
}

function clickMenuClientes() {
    window.location.href = "clientes.html";
}

function ativarMenuAtual() {
    document.querySelectorAll('.item-menu').forEach(item => {
        item.classList.remove('ativo');
    });

    const paginaAtual = window.location.pathname.split("/").pop();

    let idMenu = '';

    switch(paginaAtual) {
        case 'index.html':
        case '':
            idMenu = 'menuInicio';
            break;
        case 'novaVenda.html':
            idMenu = 'menuVenda';
            break;
        case 'produtos.html':
            idMenu = 'menuProdutos';
            break;
        case 'fiado.html':
            idMenu = 'menuFiado';
            break;
        case 'estoque.html':
            idMenu = 'menuEstoque';
            break;
        case 'clientes.html':
            idMenu = 'menuClientes';
            break;
        default:
            idMenu = 'menuInicio';
    }

    const menuAtivo = document.getElementById(idMenu);
    if (menuAtivo) {
        menuAtivo.classList.add('ativo');
    }
}

// ==================== INTERCEPTOR DE REQUISIÇÕES ====================
// Monitora todas as requisições fetch e faz logout automático em caso de 401

const originalFetch2 = window.fetch;
window.fetch = function(...args) {
    const token = localStorage.getItem('accessToken');
    const url = args[0];
    
    // Adicionar token nas chamadas da API
    if (token && typeof url === 'string' && url.includes(API_BASE_URL)) {
        const options = args[1] || {};
        options.headers = options.headers || {};
        options.headers['Authorization'] = `Bearer ${token}`;
        args[1] = options;
    }
    
    return originalFetch2.apply(this, args).then(async response => {
        // Se receber 401 (não autorizado), fazer logout automático
        if (response.status === 401) {
            console.log('Token inválido ou expirado. Fazendo logout automático...');
            
            // Limpar localStorage
            localStorage.removeItem('accessToken');
            localStorage.removeItem('refreshToken');
            localStorage.removeItem('username');
            localStorage.removeItem('userRole');
            localStorage.removeItem('userTipo');
            localStorage.removeItem('tokenExpiresAt');
            
            // Redirecionar para login
            window.location.href = 'login.html';
            
            throw new Error('Sessão expirada. Faça login novamente.');
        }
        
        return response;
    });
};

// ==================== MODAL CLIENTE ====================
const modal = document.getElementById("modalCadastro");

function openNewClient(){
    const modal = document.getElementById("modalCadastro");
    if (modal) {
        // Resetar formulário
        const form = modal.querySelector('form');
        if (form) {
            form.reset();
            form.onsubmit = salvarCliente;
        }
        const titulo = modal.querySelector('h2');
        if (titulo) titulo.textContent = 'Novo Cliente';
        
        modal.style.display = "block";
    }
}

function closeNewClient(){
    if (modal) modal.style.display = "none";
}

// Fecha se clicar fora da caixa
window.onclick = function(event){
    if (event.target == modal){
        closeNewClient();
    }
} 

// ==================== MODAL PRODUTO ====================
function openNewProduct() {
    const modalProduto = document.getElementById('modalProduto');
    if (modalProduto) {
        const form = document.getElementById('formNovoProduto');
        if (form) {
            form.reset();
            form.onsubmit = salvarProduto;
        }
        const titulo = modalProduto.querySelector('h2');
        if (titulo) titulo.textContent = 'Novo Produto';
        
        modalProduto.style.display = 'flex';
    }
}

function closeNewProduct() {
    const modalProduto = document.getElementById('modalProduto');
    if (modalProduto) {
        modalProduto.style.display = 'none';
        fecharModalProduto();
    }
}


const API_BASE_URL = 'https://localhost:7125/api';

// ==================== DASHBOARD ====================

// Função principal para carregar os dados do dashboard
async function carregarDashboard() {
    try {
        console.log('Carregando dashboard...');
        
        const response = await fetch(`${API_BASE_URL}/Dashboard`);
        
        if (!response.ok) {
            throw new Error(`Erro HTTP: ${response.status}`);
        }
        
        const dados = await response.json();
        console.log('Dados recebidos da API:', dados);
        
        // Atualizar os cards do dashboard
        atualizarCardsDashboard(dados);
        
        // Atualizar a lista de últimas vendas
        atualizarUltimasVendas(dados.ultimasVendas);
        
    } catch (error) {
        console.error('Erro ao carregar dashboard:', error);
        mostrarErroDashboard();
    }
}

// Função para atualizar os 4 cards principais
function atualizarCardsDashboard(dados) {
    // Card 1: Vendas Hoje
    const cardVendas = document.querySelector('.grade-cards .card-info:first-child');
    if (cardVendas) {
        const valorCard = cardVendas.querySelector('.valor-card');
        const subtextoCard = cardVendas.querySelector('.subtexto-card');
        
        if (valorCard) {
            valorCard.textContent = dados.vendasHoje.quantidadeVendas;
            console.log('Card Vendas atualizado:', dados.vendasHoje.quantidadeVendas);
        }
        if (subtextoCard) {
            subtextoCard.textContent = `R$ ${dados.vendasHoje.totalValor.toFixed(2)}`;
        }
    }
    
    // Card 2: Total Fiado
    const cardFiado = document.querySelector('.grade-cards .card-info:nth-child(2)');
    if (cardFiado) {
        const valorCard = cardFiado.querySelector('.valor-card');
        if (valorCard) {
            valorCard.textContent = `R$ ${dados.totalFiado.toFixed(2)}`;
            console.log('Card Fiado atualizado:', dados.totalFiado);
        }
    }
    
    // Card 3: Produtos em Baixo Estoque
    const cardBaixoEstoque = document.querySelector('.grade-cards .card-info:nth-child(3)');
    if (cardBaixoEstoque) {
        const valorCard = cardBaixoEstoque.querySelector('.valor-card');
        if (valorCard) {
            valorCard.textContent = dados.produtosBaixoEstoque;
            console.log('Card Baixo Estoque atualizado:', dados.produtosBaixoEstoque);
        }
    }
    
    // Card 4: Clientes Cadastrados
    const cardClientes = document.querySelector('.grade-cards .card-info:nth-child(4)');
    if (cardClientes) {
        const valorCard = cardClientes.querySelector('.valor-card');
        if (valorCard) {
            valorCard.textContent = dados.totalClientes;
            console.log('Card Clientes atualizado:', dados.totalClientes);
        }
    }
}

// Função para atualizar a lista de últimas vendas
function atualizarUltimasVendas(vendas) {
    const blocoVendas = document.querySelector('.bloco-vendas');
    if (!blocoVendas) {
        console.error('Elemento .bloco-vendas não encontrado');
        return;
    }
    
    if (!vendas || vendas.length === 0) {
        blocoVendas.innerHTML = `
            <h3>Últimas Vendas</h3>
            <p class="vazio">Nenhuma venda registrada</p>
        `;
        return;
    }
    
    // Criar lista HTML das vendas
    let listaVendasHtml = '<h3>Últimas Vendas</h3>';
    listaVendasHtml += '<div style="max-height: 300px; overflow-y: auto;">';
    
    vendas.forEach(venda => {
        const data = new Date(venda.dataHora);
        const dataFormatada = data.toLocaleDateString('pt-BR');
        const horaFormatada = data.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
        
        listaVendasHtml += `
            <div class="venda-item" style="
                padding: 12px;
                border-bottom: 1px solid #e5e7eb;
                display: flex;
                justify-content: space-between;
                align-items: center;
            ">
                <div>
                    <strong>${venda.nomeCliente || 'Cliente não identificado'}</strong>
                    <p style="font-size: 0.75rem; color: #6b7280; margin-top: 4px;">
                        ${dataFormatada} às ${horaFormatada}
                    </p>
                </div>
                <span style="font-weight: bold; color: #10b981;">
                    R$ ${venda.valorTotal.toFixed(2)}
                </span>
            </div>
        `;
    });
    
    listaVendasHtml += '</div>';
    blocoVendas.innerHTML = listaVendasHtml;
    console.log('Últimas vendas atualizadas:', vendas.length, 'vendas');
}

// Funções auxiliares (feedback visual)
function mostrarLoadingCards() {
    const valoresCards = document.querySelectorAll('.valor-card');
    valoresCards.forEach(card => {
        const textoAtual = card.textContent;
        if (textoAtual === '0' || textoAtual === 'R$ 0,00' || textoAtual === 'R$ 0.00') {
            card.textContent = '...';
        }
    });
}

function mostrarErroDashboard() {
    const blocoVendas = document.querySelector('.bloco-vendas');
    if (blocoVendas) {
        blocoVendas.innerHTML = `
            <h3>Últimas Vendas</h3>
            <p class="vazio" style="color: #ef4444;">
                 Erro ao carregar dados. Verifique se a API está rodando em ${API_BASE_URL}
            </p>
        `;
    }
    
    // Mostrar erro nos cards
    const cardsInfo = document.querySelectorAll('.card-info');
    cardsInfo.forEach(card => {
        const valorCard = card.querySelector('.valor-card');
        if (valorCard && valorCard.textContent === '...') {
            valorCard.textContent = 'Erro';
        }
    });
}

// ==================== API DE CLIENTES ====================

// Carregar lista de clientes
async function carregarClientes() {
    try {
        const response = await fetch(`${API_BASE_URL}/Cliente`);
        
        if (!response.ok) {
            throw new Error(`Erro HTTP: ${response.status}`);
        }
        
        const clientes = await response.json();
        console.log('Clientes carregados:', clientes);
        atualizarListaClientes(clientes);
        
    } catch (error) {
        console.error('Erro ao carregar clientes:', error);
        mostrarErroClientes();
    }
}

// Atualizar a lista de clientes na tela (clientes.html)
function atualizarListaClientes(clientes) {
    const gradeClientes = document.querySelector('.grade-listagem-clientes');
    if (!gradeClientes) return;
    
    if (!clientes || clientes.length === 0) {
        gradeClientes.innerHTML = `
            <div style="text-align: center; padding: 40px; color: #9ca3af; width: 100%;">
                <p>Nenhum cliente cadastrado</p>
                <button onclick="openNewClient()" style="margin-top: 10px; padding: 8px 16px; background: #2563eb; color: white; border: none; border-radius: 6px; cursor: pointer;">
                    + Cadastrar primeiro cliente
                </button>
            </div>
        `;
        return;
    }
    
    // Gerar HTML dos cards de clientes
    let html = '';
    clientes.forEach(cliente => {
        // Tratar valores null/undefined
        const nome = cliente.nome || 'Nome não informado';
        const cpf = cliente.cpf ? formatarCpf(cliente.cpf) : 'Não informado';
        const telefone = cliente.telefone || 'Não informado';
        const endereco = cliente.endereco || 'Não informado';
        const totalFiado = cliente.totalFiado || 0;
        
        html += `
            <div class="card-perfil-cliente" data-id="${cliente.idPessoa}">
                <div class="topo-card-cliente">
                    <div class="info-nome">
                        <div class="avatar-cliente"><i class="fa-solid fa-circle-user"></i></div>
                        <strong>${escapeHtml(nome)}</strong>
                    </div>
                    <div class="acoes-tabela">
                        <button class="botao-acao-tabela" onclick="editarCliente(${cliente.idPessoa})">
                            <i class="fa-solid fa-pen-to-square"></i>
                        </button>
                        <button class="botao-acao-tabela excluir" onclick="excluirCliente(${cliente.idPessoa})">
                            <i class="fa-solid fa-trash-can"></i>
                        </button>
                    </div>
                </div>
                <div class="detalhes-cliente">
                    <p><i class="fa-solid fa-id-card"></i> ${cpf}</p>
                    <p><i class="fa-solid fa-phone"></i> ${telefone}</p>
                    <p><i class="fa-solid fa-location-dot"></i> ${endereco}</p>
                    ${totalFiado > 0 ? `<p class="texto-laranja"><i class="fa-solid fa-sack-dollar"></i> Débito: R$ ${totalFiado.toFixed(2)}</p>` : ''}
                </div>
            </div>
        `;
    });
    
    gradeClientes.innerHTML = html;
}

// Salvar novo cliente
async function salvarCliente(event) {
    event.preventDefault();
    
    const form = event.target;
    
    // Capturar os campos na ordem correta do HTML
    const inputs = form.querySelectorAll('input');
    
    // Mapeamento correto dos campos
    const nome = inputs[0]?.value;      // Nome Completo
    const cpf = inputs[1]?.value;       // CPF
    const telefone = inputs[2]?.value;  // Telefone (novo campo)
    const endereco = inputs[3]?.value;  // Endereço
    
    // Validar campos obrigatórios
    if (!nome || !cpf) {
        alert('Nome e CPF são obrigatórios!');
        return;
    }
    
    const clienteData = {
        nome: nome,
        cpf: cpf.replace(/\D/g, ''), // Remove caracteres não numéricos
        telefone: telefone ? telefone.replace(/\D/g, '') : '', // Tratar telefone opcional
        endereco: endereco || '',
        isCliente: true,
        isFuncionario: false,
        status: "ATIVO"
    };
    
    try {
        const response = await fetch(`${API_BASE_URL}/Cliente`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(clienteData)
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.mensagem || 'Erro ao salvar cliente');
        }
        
        alert('Cliente cadastrado com sucesso!');
        closeNewClient();
        carregarClientes(); // Recarregar lista
        carregarDashboard(); // Atualizar dashboard se estiver na página inicial
        atualizarEstatisticasClientes();
        
        // Limpar formulário
        form.reset();
        
    } catch (error) {
        console.error('Erro ao salvar cliente:', error);
        alert('Erro ao salvar cliente: ' + error.message);
    }
}

// Editar cliente
async function editarCliente(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/Cliente/${id}`);
        
        if (!response.ok) {
            throw new Error('Cliente não encontrado');
        }
        
        const cliente = await response.json();
        
        // Abrir modal de edição (reutilizar o mesmo modal)
        const modal = document.getElementById('modalCadastro');
        if (!modal) {
            alert('Modal não encontrado');
            return;
        }
        
        // Preencher formulário com dados do cliente
        const form = modal.querySelector('form');
        const inputs = form.querySelectorAll('input');
        
        // Mapeamento correto dos campos
        if (inputs[0]) inputs[0].value = cliente.nome || '';      // Nome
        if (inputs[1]) inputs[1].value = formatarCpf(cliente.cpf || ''); // CPF
        if (inputs[2]) inputs[2].value = cliente.telefone || '';  // Telefone
        if (inputs[3]) inputs[3].value = cliente.endereco || '';  // Endereço
        
        // Mudar título do modal
        const titulo = modal.querySelector('h2');
        if (titulo) titulo.textContent = 'Editar Cliente';
        
        // Mudar action do form
        form.onsubmit = (e) => atualizarCliente(e, id);
        
        // Adicionar botão de cancelar customizado
        const btnCancelar = modal.querySelector('.btn-cancel');
        if (btnCancelar) {
            btnCancelar.onclick = () => {
                fecharModalEdicao();
                atualizarEstatisticasClientes();
            };
        }
        
        modal.style.display = 'block';
        
    } catch (error) {
        console.error('Erro ao carregar cliente:', error);
        alert('Erro ao carregar dados do cliente');
    }
}

// Atualizar cards de estatísticas dos clientes
async function atualizarEstatisticasClientes() {
    try {
        // Buscar todos os clientes
        const response = await fetch(`${API_BASE_URL}/Cliente`);
        
        if (!response.ok) {
            throw new Error('Erro ao buscar clientes');
        }
        
        const clientes = await response.json();
        
        // Calcular estatísticas
        const totalClientes = clientes.length;
        const clientesDebito = clientes.filter(c => (c.totalFiado || 0) > 0).length;
        const totalReceber = clientes.reduce((sum, c) => sum + (c.totalFiado || 0), 0);
        
        // Atualizar os cards
        const totalClientesSpan = document.getElementById('totalClientes');
        const clientesDebitoSpan = document.getElementById('clientesDebito');
        const totalReceberSpan = document.getElementById('totalReceber');
        
        if (totalClientesSpan) totalClientesSpan.textContent = totalClientes;
        if (clientesDebitoSpan) clientesDebitoSpan.textContent = clientesDebito;
        if (totalReceberSpan) totalReceberSpan.textContent = `R$ ${totalReceber.toFixed(2)}`;
        
        console.log('Estatísticas atualizadas:', { totalClientes, clientesDebito, totalReceber });
        
    } catch (error) {
        console.error('Erro ao atualizar estatísticas:', error);
    }
}

// Atualizar cliente
async function atualizarCliente(event, id) {
    event.preventDefault();
    
    const form = event.target;
    const inputs = form.querySelectorAll('input');
    
    // Mapeamento correto dos campos
    const nome = inputs[0]?.value;
    const cpf = inputs[1]?.value.replace(/\D/g, '');
    const telefone = inputs[2]?.value.replace(/\D/g, '');
    const endereco = inputs[3]?.value;
    
    const clienteData = {
        nome: nome,
        cpf: cpf,
        telefone: telefone || '',
        endereco: endereco || '',
        isCliente: true,
        isFuncionario: false,
        status: "ATIVO"
    };
    
    try {
        const response = await fetch(`${API_BASE_URL}/Cliente/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(clienteData)
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.mensagem || 'Erro ao atualizar cliente');
        }
        
        alert('Cliente atualizado com sucesso!');
        fecharModalEdicao();
        carregarClientes();
        atualizarEstatisticasClientes();
        
    } catch (error) {
        console.error('Erro ao atualizar cliente:', error);
        alert('Erro ao atualizar cliente: ' + error.message);
    }
}

// Excluir cliente
async function excluirCliente(id) {
    if (!confirm('Tem certeza que deseja excluir este cliente?\nEsta ação não pode ser desfeita.')) {
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/Cliente/${id}`, {
            method: 'DELETE'
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.mensagem || 'Erro ao excluir cliente');
        }
        
        alert('Cliente excluído com sucesso!');
        carregarClientes();
        atualizarEstatisticasClientes(); // Atualizar estatísticas
        
    } catch (error) {
        console.error('Erro ao excluir cliente:', error);
        alert('Erro ao excluir cliente: ' + error.message);
    }
}

// Buscar clientes por termo
async function buscarClientes(termo) {
    if (!termo || termo.length < 2) {
        carregarClientes();
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/Cliente/buscar?termo=${encodeURIComponent(termo)}`);
        
        if (!response.ok) {
            throw new Error('Erro na busca');
        }
        
        const clientes = await response.json();
        atualizarListaClientes(clientes);
        
    } catch (error) {
        console.error('Erro ao buscar clientes:', error);
    }
}

// Funções auxiliares
function formatarCpf(cpf) {
    if (!cpf) return 'Não informado';
    const str = cpf.toString();
    if (str.length === 11) {
        return str.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
    }
    return str;
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function fecharModalEdicao() {
    const modal = document.getElementById('modalCadastro');
    if (modal) {
        modal.style.display = 'none';
        // Resetar formulário
        const form = modal.querySelector('form');
        if (form) {
            form.reset();
            form.onsubmit = salvarCliente;
        }
        const titulo = modal.querySelector('h2');
        if (titulo) titulo.textContent = 'Novo Cliente';
    }
}

function mostrarErroClientes() {
    const gradeClientes = document.querySelector('.grade-listagem-clientes');
    if (gradeClientes) {
        gradeClientes.innerHTML = `
            <div style="text-align: center; padding: 40px; color: #ef4444; width: 100%;">
                <p>Erro ao carregar clientes</p>
                <p style="font-size: 12px; margin-top: 10px;">Verifique se a API está rodando em ${API_BASE_URL}</p>
            </div>
        `;
    }
}

// ==================== API DE CATEGORIAS ====================

// Carregar categorias da API
async function carregarCategorias() {
    try {
        const response = await fetch(`${API_BASE_URL}/Categoria`);
        
        if (!response.ok) {
            throw new Error(`Erro HTTP: ${response.status}`);
        }
        
        const categorias = await response.json();
        console.log('Categorias carregadas:', categorias);
        atualizarSelectCategorias(categorias);
        
    } catch (error) {
        console.error('Erro ao carregar categorias:', error);
        // Fallback: categorias padrão
        const categoriasPadrao = [
            { idCategoria: 1, descricao: "Bebidas" },
            { idCategoria: 2, descricao: "Gás" },
            { idCategoria: 3, descricao: "Churrasco" },
            { idCategoria: 4, descricao: "Limpeza" },
            { idCategoria: 5, descricao: "Variedades" }
        ];
        atualizarSelectCategorias(categoriasPadrao);
    }
}

// Atualizar o select de categorias no modal
function atualizarSelectCategorias(categorias) {
    const selectCategoria = document.getElementById('produtoCategoria');
    if (!selectCategoria) return;
    
    selectCategoria.innerHTML = '';
    
    categorias.forEach(categoria => {
        const option = document.createElement('option');
        option.value = categoria.idCategoria;
        option.textContent = categoria.descricao;
        selectCategoria.appendChild(option);
    });
}

// ==================== API DE CATEGORIAS ====================

// Carregar categorias da API
async function carregarCategorias() {
    try {
        const response = await fetch(`${API_BASE_URL}/Categoria`);
        
        if (!response.ok) {
            throw new Error(`Erro HTTP: ${response.status}`);
        }
        
        const categorias = await response.json();
        console.log('Categorias carregadas:', categorias);
        atualizarSelectCategorias(categorias);
        return categorias;
        
    } catch (error) {
        console.error('Erro ao carregar categorias:', error);
        // Fallback: categorias padrão
        const categoriasPadrao = [
            { idCategoria: 1, descricao: "Bebidas" },
            { idCategoria: 2, descricao: "Gás" },
            { idCategoria: 3, descricao: "Churrasco" },
            { idCategoria: 4, descricao: "Limpeza" },
            { idCategoria: 5, descricao: "Variedades" }
        ];
        atualizarSelectCategorias(categoriasPadrao);
        return categoriasPadrao;
    }
}

// Atualizar o select de categorias no modal
function atualizarSelectCategorias(categorias) {
    const selectCategoria = document.getElementById('produtoCategoria');
    if (!selectCategoria) return;
    
    selectCategoria.innerHTML = '';
    
    categorias.forEach(categoria => {
        const option = document.createElement('option');
        option.value = categoria.idCategoria;
        option.textContent = categoria.descricao;
        selectCategoria.appendChild(option);
    });
}

// ==================== API DE ESTOQUE ====================

// Carregar resumo do estoque (cards)
async function carregarResumoEstoque() {
    try {
        const response = await fetch(`${API_BASE_URL}/Estoque/resumo`);
        
        if (!response.ok) {
            throw new Error(`Erro HTTP: ${response.status}`);
        }
        
        const resumo = await response.json();
        console.log('Resumo do estoque:', resumo);
        
        // Atualizar cards
        const totalProdutosSpan = document.getElementById('totalProdutos');
        const baixoEstoqueSpan = document.getElementById('baixoEstoque');
        const valorTotalSpan = document.getElementById('valorTotalEstoque');
        
        if (totalProdutosSpan) totalProdutosSpan.textContent = resumo.totalProdutos || 0;
        if (baixoEstoqueSpan) baixoEstoqueSpan.textContent = resumo.produtosBaixoEstoque || 0;
        if (valorTotalSpan) valorTotalSpan.textContent = `R$ ${(resumo.valorTotalEstoque || 0).toFixed(2)}`;
        
    } catch (error) {
        console.error('Erro ao carregar resumo do estoque:', error);
        // Fallback: calcular localmente
        await carregarResumoEstoqueLocal();
    }
}

// Fallback: calcular resumo localmente a partir dos produtos
async function carregarResumoEstoqueLocal() {
    try {
        const response = await fetch(`${API_BASE_URL}/Produto`);
        if (!response.ok) throw new Error('Erro ao buscar produtos');
        
        const produtos = await response.json();
        
        const totalProdutos = produtos.length;
        const produtosBaixoEstoque = produtos.filter(p => p.qtde < 10).length;
        const valorTotalEstoque = produtos.reduce((sum, p) => sum + (p.precoVenda * p.qtde), 0);
        
        const totalProdutosSpan = document.getElementById('totalProdutos');
        const baixoEstoqueSpan = document.getElementById('baixoEstoque');
        const valorTotalSpan = document.getElementById('valorTotalEstoque');
        
        if (totalProdutosSpan) totalProdutosSpan.textContent = totalProdutos;
        if (baixoEstoqueSpan) baixoEstoqueSpan.textContent = produtosBaixoEstoque;
        if (valorTotalSpan) valorTotalSpan.textContent = `R$ ${valorTotalEstoque.toFixed(2)}`;
        
    } catch (error) {
        console.error('Erro no fallback do estoque:', error);
    }
}

// Carregar tabela de estoque
async function carregarTabelaEstoque(filtroCategoria = 'todas', busca = '') {
    try {
        const response = await fetch(`${API_BASE_URL}/Produto`);
        
        if (!response.ok) {
            throw new Error(`Erro HTTP: ${response.status}`);
        }
        
        let produtos = await response.json();
        
        // Aplicar filtro de categoria
        if (filtroCategoria !== 'todas') {
            produtos = produtos.filter(p => p.categoriaDescricao === filtroCategoria);
        }
        
        // Aplicar busca por nome
        if (busca && busca.trim() !== '') {
            const termo = busca.toLowerCase();
            produtos = produtos.filter(p => p.descricao.toLowerCase().includes(termo));
        }
        
        atualizarTabelaEstoque(produtos);
        
    } catch (error) {
        console.error('Erro ao carregar tabela de estoque:', error);
        mostrarErroTabelaEstoque();
    }
}

// Atualizar tabela de estoque no HTML
function atualizarTabelaEstoque(produtos) {
    const tbody = document.getElementById('tabelaEstoqueBody');
    if (!tbody) return;
    
    if (!produtos || produtos.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="6" style="text-align: center; padding: 40px;">
                    Nenhum produto encontrado
                </td>
            </tr>
        `;
        return;
    }
    
    let html = '';
    produtos.forEach(produto => {
        const statusEstoque = produto.qtde < 10 ? 'texto-laranja' : 'texto-verde';
        const valorTotal = produto.precoVenda * produto.qtde;
        const statusClass = produto.qtde < 10 ? 'status-alerta' : 'status-normal';
        const statusTexto = produto.qtde < 10 ? 'Baixo Estoque' : 'Normal';
        
        html += `
            <tr>
                <td>
                    <strong>${escapeHtml(produto.descricao)}</strong><br>
                    <span class="sub-texto">unidade</span>
                </td>
                <td><span class="etiqueta">${escapeHtml(produto.categoriaDescricao || 'Sem categoria')}</span></td>
                <td>R$ ${produto.precoVenda.toFixed(2)}</td>
                <td class="${statusEstoque}">${produto.qtde} un.</td>
                <td>R$ ${valorTotal.toFixed(2)}</td>
                <td><span class="status ${statusClass}">${statusTexto}</span></td>
            </tr>
        `;
    });
    
    tbody.innerHTML = html;
}

// Carregar botões de filtro por categoria
async function carregarFiltrosCategoria() {
    try {
        const response = await fetch(`${API_BASE_URL}/Categoria`);
        
        if (!response.ok) {
            throw new Error(`Erro HTTP: ${response.status}`);
        }
        
        const categorias = await response.json();
        const container = document.getElementById('filtrosCategoria');
        if (!container) return;
        
        // Limpar container (manter apenas o botão "Todas")
        container.innerHTML = '<button class="filtro ativo" data-categoria="todas">Todas</button>';
        
        // Adicionar botões para cada categoria
        categorias.forEach(categoria => {
            const btn = document.createElement('button');
            btn.className = 'filtro';
            btn.setAttribute('data-categoria', categoria.descricao);
            btn.textContent = categoria.descricao;
            btn.onclick = () => aplicarFiltroCategoria(categoria.descricao);
            container.appendChild(btn);
        });
        
        // Remover classe 'ativo' de todos e adicionar no "Todas"
        const botoes = container.querySelectorAll('.filtro');
        botoes.forEach(btn => btn.classList.remove('ativo'));
        botoes[0].classList.add('ativo');
        
    } catch (error) {
        console.error('Erro ao carregar filtros:', error);
        // Fallback: filtros padrão
        const container = document.getElementById('filtrosCategoria');
        if (container && container.children.length <= 1) {
            const categoriasPadrao = ['Bebidas', 'Gás', 'Churrasco', 'Limpeza', 'Variedades'];
            categoriasPadrao.forEach(cat => {
                const btn = document.createElement('button');
                btn.className = 'filtro';
                btn.setAttribute('data-categoria', cat);
                btn.textContent = cat;
                btn.onclick = () => aplicarFiltroCategoria(cat);
                container.appendChild(btn);
            });
        }
    }
}

// Aplicar filtro de categoria
let filtroAtual = 'todas';
let buscaAtual = '';

function aplicarFiltroCategoria(categoria) {
    filtroAtual = categoria;
    
    // Atualizar UI dos botões
    const botoes = document.querySelectorAll('.filtro');
    botoes.forEach(btn => {
        if (btn.getAttribute('data-categoria') === categoria) {
            btn.classList.add('ativo');
        } else {
            btn.classList.remove('ativo');
        }
    });
    
    // Recarregar tabela com filtros atuais
    carregarTabelaEstoque(filtroAtual, buscaAtual);
}

// Aplicar busca
function aplicarBusca() {
    const inputBusca = document.getElementById('buscaProduto');
    if (inputBusca) {
        buscaAtual = inputBusca.value;
        carregarTabelaEstoque(filtroAtual, buscaAtual);
    }
}

// Mostrar erro na tabela
function mostrarErroTabelaEstoque() {
    const tbody = document.getElementById('tabelaEstoqueBody');
    if (tbody) {
        tbody.innerHTML = `
            <tr>
                <td colspan="6" style="text-align: center; padding: 40px; color: #ef4444;">
                    Erro ao carregar produtos
                    <br>
                    <span style="font-size: 12px;">Verifique se a API está rodando em ${API_BASE_URL}</span>
                </td>
            </tr>
        `;
    }
}

// ==================== API DE PRODUTOS ====================

// Carregar lista de produtos
async function carregarProdutos() {
    try {
        const response = await fetch(`${API_BASE_URL}/Produto`);
        
        if (!response.ok) {
            throw new Error(`Erro HTTP: ${response.status}`);
        }
        
        const produtos = await response.json();
        console.log('Produtos carregados:', produtos);
        atualizarTabelaProdutos(produtos);
        
    } catch (error) {
        console.error('Erro ao carregar produtos:', error);
        mostrarErroProdutos();
    }
}

// Atualizar tabela de produtos no HTML
function atualizarTabelaProdutos(produtos) {
    const tbody = document.querySelector('.tabela-listagem tbody');
    if (!tbody) return;
    
    if (!produtos || produtos.length === 0) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5" style="text-align: center; padding: 40px;">
                    Nenhum produto cadastrado
                    <br>
                    <button onclick="openNewProduct()" style="margin-top: 10px; padding: 8px 16px; background: #2563eb; color: white; border: none; border-radius: 6px; cursor: pointer;">
                        + Cadastrar primeiro produto
                    </button>
                </td>
            </tr>
        `;
        return;
    }
    
    let html = '';
    produtos.forEach(produto => {
        const statusEstoque = produto.qtde < 10 ? 'texto-laranja' : 'texto-verde';
        
        html += `
            <tr data-id="${produto.idProduto}">
                <td>
                    <strong>${escapeHtml(produto.descricao)}</strong><br>
                    <span class="sub-texto">unidade</span>
                </td>
                <td>${produto.categoriaDescricao || 'Sem categoria'}</td>
                <td>R$ ${produto.precoVenda.toFixed(2)}</td>
                <td class="${statusEstoque}">${produto.qtde} un.</td>
                <td>
                    <div class="acoes-tabela">
                        <button class="botao-acao-tabela" onclick="editarProduto(${produto.idProduto})">
                            <i class="fa-solid fa-pen-to-square"></i>
                        </button>
                        <button class="botao-acao-tabela excluir" onclick="excluirProduto(${produto.idProduto})">
                            <i class="fa-solid fa-trash-can"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
    });
    
    tbody.innerHTML = html;
}

// Salvar novo produto
async function salvarProduto(event) {
    event.preventDefault();
    
    // Capturar valores do formulário pelos IDs
    const nome = document.getElementById('produtoNome')?.value;
    const categoriaId = parseInt(document.getElementById('produtoCategoria')?.value);
    const precoVenda = parseFloat(document.getElementById('produtoPreco')?.value);
    const qtde = parseInt(document.getElementById('produtoEstoque')?.value);
    
    // Validar campos obrigatórios
    if (!nome || nome.trim() === '') {
        alert('Nome do produto é obrigatório!');
        return;
    }
    
    if (!categoriaId || isNaN(categoriaId)) {
        alert('Selecione uma categoria!');
        return;
    }
    
    if (!precoVenda || precoVenda <= 0) {
        alert('Preço do produto é obrigatório e deve ser maior que zero!');
        return;
    }
    
    const produtoData = {
        descricao: nome.trim(),
        precoCompra: precoVenda * 0.7, // Valor aproximado (70% do preço de venda)
        precoVenda: precoVenda,
        qtde: qtde || 0,
        idCategoria: categoriaId
    };
    
    try {
        const response = await fetch(`${API_BASE_URL}/Produto`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(produtoData)
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.mensagem || error.title || 'Erro ao salvar produto');
        }
        
        alert('Produto cadastrado com sucesso!');
        closeNewProduct();
        carregarProdutos();
        
        // Atualizar dashboard se disponível
        if (typeof carregarDashboard === 'function') {
            carregarDashboard();
        }
        
    } catch (error) {
        console.error('Erro ao salvar produto:', error);
        alert('Erro ao salvar produto: ' + error.message);
    }
}

// Editar produto
async function editarProduto(id) {
    try {
        const response = await fetch(`${API_BASE_URL}/Produto/${id}`);
        
        if (!response.ok) {
            throw new Error('Produto não encontrado');
        }
        
        const produto = await response.json();
        
        const modal = document.getElementById('modalProduto');
        if (!modal) {
            alert('Modal não encontrado');
            return;
        }
        
        // Preencher formulário com dados do produto
        const nomeInput = document.getElementById('produtoNome');
        const precoInput = document.getElementById('produtoPreco');
        const estoqueInput = document.getElementById('produtoEstoque');
        const categoriaSelect = document.getElementById('produtoCategoria');
        
        if (nomeInput) nomeInput.value = produto.descricao || '';
        if (precoInput) precoInput.value = produto.precoVenda || 0;
        if (estoqueInput) estoqueInput.value = produto.qtde || 0;
        
        // Selecionar categoria
        if (categoriaSelect && produto.idCategoria) {
            categoriaSelect.value = produto.idCategoria;
        }
        
        const titulo = modal.querySelector('h2');
        if (titulo) titulo.textContent = 'Editar Produto';
        
        // Mudar ação do formulário
        const form = document.getElementById('formNovoProduto');
        if (form) {
            form.onsubmit = (e) => atualizarProduto(e, id);
        }
        
        modal.style.display = 'flex';
        
    } catch (error) {
        console.error('Erro ao carregar produto:', error);
        alert('Erro ao carregar dados do produto: ' + error.message);
    }
}

// Atualizar produto
async function atualizarProduto(event, id) {
    event.preventDefault();
    
    const nome = document.getElementById('produtoNome')?.value;
    const categoriaId = parseInt(document.getElementById('produtoCategoria')?.value);
    const precoVenda = parseFloat(document.getElementById('produtoPreco')?.value);
    const qtde = parseInt(document.getElementById('produtoEstoque')?.value);
    
    if (!nome || nome.trim() === '') {
        alert('Nome do produto é obrigatório!');
        return;
    }
    
    if (!categoriaId || isNaN(categoriaId)) {
        alert('Selecione uma categoria!');
        return;
    }
    
    const produtoData = {
        descricao: nome.trim(),
        precoCompra: precoVenda * 0.7,
        precoVenda: precoVenda,
        qtde: qtde || 0,
        idCategoria: categoriaId
    };
    
    try {
        const response = await fetch(`${API_BASE_URL}/Produto/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(produtoData)
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.mensagem || 'Erro ao atualizar produto');
        }
        
        alert('Produto atualizado com sucesso!');
        closeNewProduct();
        carregarProdutos();
        
    } catch (error) {
        console.error('Erro ao atualizar produto:', error);
        alert('Erro ao atualizar produto: ' + error.message);
    }
}

// Excluir produto
async function excluirProduto(id) {
    if (!confirm('Tem certeza que deseja excluir este produto?\nEsta ação não pode ser desfeita.')) {
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/Produto/${id}`, {
            method: 'DELETE'
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.mensagem || 'Erro ao excluir produto');
        }
        
        alert('Produto excluído com sucesso!');
        carregarProdutos();
        
        if (typeof carregarDashboard === 'function') {
            carregarDashboard();
        }
        
    } catch (error) {
        console.error('Erro ao excluir produto:', error);
        alert('Erro ao excluir produto: ' + error.message);
    }
}

// Funções do modal de produto
function openNewProduct() {
    const modal = document.getElementById('modalProduto');
    if (modal) {
        // Recarregar categorias para garantir dados atualizados
        carregarCategorias();
        
        // Resetar formulário
        const form = document.getElementById('formNovoProduto');
        if (form) {
            form.reset();
            form.onsubmit = salvarProduto;
        }
        
        // Resetar valores padrão
        const nomeInput = document.getElementById('produtoNome');
        const precoInput = document.getElementById('produtoPreco');
        const estoqueInput = document.getElementById('produtoEstoque');
        
        if (nomeInput) nomeInput.value = '';
        if (precoInput) precoInput.value = '0';
        if (estoqueInput) estoqueInput.value = '0';
        
        const titulo = modal.querySelector('h2');
        if (titulo) titulo.textContent = 'Novo Produto';
        
        modal.style.display = 'flex';
    }
}

function closeNewProduct() {
    const modal = document.getElementById('modalProduto');
    if (modal) {
        modal.style.display = 'none';
    }
}

function mostrarErroProdutos() {
    const tbody = document.querySelector('.tabela-listagem tbody');
    if (tbody) {
        tbody.innerHTML = `
            <tr>
                <td colspan="5" style="text-align: center; padding: 40px; color: #ef4444;">
                    Erro ao carregar produtos
                    <br>
                    <span style="font-size: 12px;">Verifique se a API está rodando em ${API_BASE_URL}</span>
                </td>
            </tr>
        `;
    }
}

// ==================== AUTENTICAÇÃO ====================

// Função de login
async function entrar() {
    const username = document.getElementById('login-usuario')?.value;
    const senha = document.getElementById('login-senha')?.value;
    
    if (!username || !senha) {
        alert('Preencha usuário e senha!');
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/Auth/login`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                username: username,
                senha: senha
            })
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.mensagem || 'Usuário ou senha inválidos');
        }
        
        const data = await response.json();
        console.log('Login realizado com sucesso:', data);
        
        // Salvar token no localStorage
        localStorage.setItem('accessToken', data.accessToken);
        localStorage.setItem('refreshToken', data.refreshToken);
        localStorage.setItem('username', data.username);
        localStorage.setItem('userRole', data.role);
        localStorage.setItem('userTipo', data.tipo);
        localStorage.setItem('tokenExpiresAt', data.expiresAt);
        
        // Redirecionar para o dashboard
        window.location.href = 'index.html';
        
    } catch (error) {
        console.error('Erro no login:', error);
        alert('Erro ao fazer login: ' + error.message);
    }
}

// Verificar se o usuário está autenticado
function isAuthenticated() {
    const token = localStorage.getItem('accessToken');
    const expiresAt = localStorage.getItem('tokenExpiresAt');
    
    if (!token) return false;
    
    // Verificar se o token expirou
    if (expiresAt) {
        const expDate = new Date(expiresAt);
        const now = new Date();
        if (expDate < now) {
            // Token expirado, tentar refresh
            refreshToken();
            return false;
        }
    }
    
    return true;
}

// Renovar token
async function refreshToken() {
    const refreshToken = localStorage.getItem('refreshToken');
    if (!refreshToken) return false;
    
    try {
        const response = await fetch(`${API_BASE_URL}/Auth/refresh`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ refreshToken: refreshToken })
        });
        
        if (!response.ok) throw new Error('Refresh token inválido');
        
        const data = await response.json();
        
        localStorage.setItem('accessToken', data.accessToken);
        localStorage.setItem('refreshToken', data.refreshToken);
        localStorage.setItem('tokenExpiresAt', data.expiresAt);
        
        return true;
        
    } catch (error) {
        console.error('Erro ao renovar token:', error);
        logout();
        return false;
    }
}

// Função de logout
function logout() {
    // Chamar API de logout
    const token = localStorage.getItem('accessToken');
    if (token) {
        fetch(`${API_BASE_URL}/Auth/logout`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        }).catch(console.error);
    }
    
    // Limpar localStorage
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('username');
    localStorage.removeItem('userRole');
    localStorage.removeItem('userTipo');
    localStorage.removeItem('tokenExpiresAt');
    
    // Redirecionar para login
    window.location.href = 'login.html';
}

// Proteger rotas que exigem autenticação
function protegerRota() {
    if (!isAuthenticated()) {
        const paginaAtual = window.location.pathname.split("/").pop();
        // Páginas que exigem autenticação
        const paginasProtegidas = ['index.html', 'clientes.html', 'produtos.html', 'estoque.html', 'fiado.html', 'novaVenda.html'];
        
        if (paginasProtegidas.includes(paginaAtual) || paginaAtual === '' || paginaAtual === '/') {
            window.location.href = 'login.html';
            return false;
        }
    }
    
    // Verificar tipo de usuário para páginas de admin
    const tipo = localStorage.getItem('userTipo');
    const paginaAtual = window.location.pathname.split("/").pop();
    
    // Páginas que só admin pode acessar (ajuste conforme necessidade)
    const paginasAdmin = ['produtos.html', 'clientes.html'];
    
    if (paginasAdmin.includes(paginaAtual) && tipo !== 'Admin') {
        alert('Acesso negado! Esta página é restrita para administradores.');
        window.location.href = 'index.html';
        return false;
    }
    
    return true;
}

// ==================== API DE VENDAS ====================

let carrinho = [];
let produtosLista = [];

// Carregar produtos para o select
async function carregarProdutosVenda() {
    try {
        const response = await fetch(`${API_BASE_URL}/Produto`);
        
        if (!response.ok) {
            throw new Error(`Erro HTTP: ${response.status}`);
        }
        
        produtosLista = await response.json();
        const select = document.getElementById('selectProduto');
        
        if (select) {
            select.innerHTML = '<option value="">Selecione um produto</option>';
            produtosLista.forEach(produto => {
                const option = document.createElement('option');
                option.value = produto.idProduto;
                option.textContent = `${produto.descricao} - R$ ${produto.precoVenda.toFixed(2)} (Estoque: ${produto.qtde})`;
                option.dataset.preco = produto.precoVenda;
                option.dataset.estoque = produto.qtde;
                select.appendChild(option);
            });
        }
        
    } catch (error) {
        console.error('Erro ao carregar produtos:', error);
    }
}

// Carregar clientes para venda fiada
async function carregarClientesVenda() {
    try {
        const response = await fetch(`${API_BASE_URL}/Cliente`);
        
        if (!response.ok) {
            throw new Error(`Erro HTTP: ${response.status}`);
        }
        
        const clientes = await response.json();
        const select = document.getElementById('clienteFiado');
        
        if (select) {
            select.innerHTML = '<option value="">Selecione um cliente</option>';
            clientes.forEach(cliente => {
                const option = document.createElement('option');
                option.value = cliente.idPessoa;
                option.textContent = `${cliente.nome} - ${cliente.cpf}`;
                select.appendChild(option);
            });
        }
        
    } catch (error) {
        console.error('Erro ao carregar clientes:', error);
    }
}

// Mostrar/esconder select de cliente baseado na forma de pagamento
function toggleClienteFiado() {
    const formaPagamento = document.getElementById('formaPagamento')?.value;
    const clienteGroup = document.getElementById('clienteFiadoGroup');
    
    if (clienteGroup) {
        clienteGroup.style.display = formaPagamento === 'Fiado' ? 'block' : 'none';
    }
}

// Adicionar produto ao carrinho
function adicionarAoCarrinho() {
    const select = document.getElementById('selectProduto');
    const produtoId = parseInt(select?.value);
    const quantidade = parseInt(document.getElementById('quantidadeProduto')?.value || 0);
    
    if (!produtoId) {
        alert('Selecione um produto!');
        return;
    }
    
    if (quantidade <= 0) {
        alert('Quantidade inválida!');
        return;
    }
    
    const produto = produtosLista.find(p => p.idProduto === produtoId);
    
    if (!produto) {
        alert('Produto não encontrado!');
        return;
    }
    
    if (produto.qtde < quantidade) {
        alert(`Estoque insuficiente! Disponível: ${produto.qtde}`);
        return;
    }
    
    // Verificar se produto já está no carrinho
    const itemExistente = carrinho.find(item => item.idProduto === produtoId);
    
    if (itemExistente) {
        const novaQuantidade = itemExistente.quantidade + quantidade;
        if (produto.qtde < novaQuantidade) {
            alert(`Estoque insuficiente! Disponível: ${produto.qtde}`);
            return;
        }
        itemExistente.quantidade = novaQuantidade;
        itemExistente.valorTotal = itemExistente.quantidade * itemExistente.precoUnitario;
    } else {
        carrinho.push({
            idProduto: produtoId,
            nome: produto.descricao,
            quantidade: quantidade,
            precoUnitario: produto.precoVenda,
            valorTotal: quantidade * produto.precoVenda,
            desconto: 0
        });
    }
    
    atualizarCarrinho();
    
    // Resetar campos
    select.value = '';
    document.getElementById('quantidadeProduto').value = '1';
}

// Atualizar visual do carrinho
function atualizarCarrinho() {
    const container = document.getElementById('carrinhoLista');
    const totalItensSpan = document.getElementById('totalItens');
    const subtotalSpan = document.getElementById('subtotal');
    const totalSpan = document.getElementById('totalVenda');
    
    const totalItens = carrinho.reduce((sum, item) => sum + item.quantidade, 0);
    const subtotal = carrinho.reduce((sum, item) => sum + item.valorTotal, 0);
    
    if (totalItensSpan) totalItensSpan.textContent = totalItens;
    if (subtotalSpan) subtotalSpan.textContent = `R$ ${subtotal.toFixed(2)}`;
    if (totalSpan) totalSpan.textContent = `R$ ${subtotal.toFixed(2)}`;
    
    if (!carrinho.length) {
        if (container) {
            container.innerHTML = '<p class="carrinho-vazio">Carrinho vazio</p>';
        }
        return;
    }
    
    let html = '<div style="max-height: 300px; overflow-y: auto;">';
    
    carrinho.forEach((item, index) => {
        html += `
            <div style="display: flex; justify-content: space-between; align-items: center; padding: 10px; border-bottom: 1px solid #e5e7eb;">
                <div style="flex: 2;">
                    <strong>${escapeHtml(item.nome)}</strong><br>
                    <small>R$ ${item.precoUnitario.toFixed(2)} x ${item.quantidade}</small>
                </div>
                <div style="flex: 1; text-align: right;">
                    <strong>R$ ${item.valorTotal.toFixed(2)}</strong>
                </div>
                <button onclick="removerDoCarrinho(${index})" style="background: #ef4444; color: white; border: none; border-radius: 4px; width: 25px; height: 25px; cursor: pointer; margin-left: 10px;">
                    <i class="fa-solid fa-trash"></i>
                </button>
            </div>
        `;
    });
    
    html += '</div>';
    
    if (container) {
        container.innerHTML = html;
    }
}

// Remover item do carrinho
function removerDoCarrinho(index) {
    carrinho.splice(index, 1);
    atualizarCarrinho();
}

// Finalizar venda
async function finalizarVenda() {
    if (!carrinho.length) {
        alert('Adicione produtos ao carrinho!');
        return;
    }
    
    const formaPagamento = document.getElementById('formaPagamento')?.value;
    const clienteId = formaPagamento === 'Fiado' ? parseInt(document.getElementById('clienteFiado')?.value) : null;
    
    if (formaPagamento === 'Fiado' && !clienteId) {
        alert('Selecione um cliente para venda fiada!');
        return;
    }
    
    const vendaData = {
        itens: carrinho.map(item => ({
            produtoId: item.idProduto,
            quantidade: item.quantidade,
            desconto: item.desconto || 0
        })),
        formaPagamento: formaPagamento,
        clienteId: clienteId
    };
    
    try {
        const response = await fetch(`${API_BASE_URL}/Venda`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(vendaData)
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.mensagem || 'Erro ao finalizar venda');
        }
        
        const result = await response.json();
        alert(`Venda finalizada com sucesso! Nº ${result.idVenda}`);
        
        // Limpar carrinho
        carrinho = [];
        atualizarCarrinho();
        
        // Recarregar produtos para atualizar estoque
        carregarProdutosVenda();
        
        // Se for fiado, recarregar fiados
        if (formaPagamento === 'Fiado' && typeof carregarFiados === 'function') {
            carregarFiados();
        }
        
    } catch (error) {
        console.error('Erro ao finalizar venda:', error);
        alert('Erro ao finalizar venda: ' + error.message);
    }
}

// Buscar produtos em tempo real
function buscarProdutosVenda() {
    const termo = document.getElementById('buscarProduto')?.value.toLowerCase();
    const select = document.getElementById('selectProduto');
    
    if (!select) return;
    
    const options = select.querySelectorAll('option');
    
    options.forEach(option => {
        if (option.value === '') return;
        const texto = option.textContent.toLowerCase();
        if (texto.includes(termo) || !termo) {
            option.style.display = '';
        } else {
            option.style.display = 'none';
        }
    });
}

// ==================== API DE FIADOS ====================

// Carregar dados de fiados
async function carregarFiados() {
    try {
        const response = await fetch(`${API_BASE_URL}/Fiado/resumo`);
        
        if (!response.ok) {
            throw new Error(`Erro HTTP: ${response.status}`);
        }
        
        const dados = await response.json();
        console.log('Dados de fiados:', dados);
        
        // Atualizar cards
        const totalReceberSpan = document.getElementById('totalReceberFiado');
        const clientesDevedoresSpan = document.getElementById('clientesDevedores');
        const vendasPendentesSpan = document.getElementById('vendasPendentes');
        
        if (totalReceberSpan) totalReceberSpan.textContent = `R$ ${(dados.totalAReceber || 0).toFixed(2)}`;
        if (clientesDevedoresSpan) clientesDevedoresSpan.textContent = dados.quantidadeClientesDevedores || 0;
        if (vendasPendentesSpan) vendasPendentesSpan.textContent = dados.quantidadeVendasPendentes || 0;
        
        // Atualizar listas
        atualizarListaClientesDebito(dados.vendasFiadas || []);
        atualizarListaVendasPendentes(dados.vendasFiadas || []);
        
    } catch (error) {
        console.error('Erro ao carregar fiados:', error);
        mostrarErroFiados();
    }
}

// Atualizar lista de clientes com débito
function atualizarListaClientesDebito(vendas) {
    const container = document.getElementById('listaClientesDebito');
    if (!container) return;
    
    // Agrupar por cliente
    const clientesMap = new Map();
    
    vendas.forEach(venda => {
        if (!clientesMap.has(venda.nomeCliente)) {
            clientesMap.set(venda.nomeCliente, {
                nome: venda.nomeCliente,
                totalDevido: 0,
                vendas: []
            });
        }
        const cliente = clientesMap.get(venda.nomeCliente);
        cliente.totalDevido += venda.valorPendente;
        cliente.vendas.push(venda);
    });
    
    const clientes = Array.from(clientesMap.values());
    
    if (!clientes.length) {
        container.innerHTML = `
            <div class="centro-vazio">
                <div class="circulo-sucesso">✓</div>
                <p>Nenhum débito pendente</p>
            </div>
        `;
        return;
    }
    
    let html = '';
    clientes.forEach(cliente => {
        html += `
            <div style="padding: 15px; border-bottom: 1px solid #e5e7eb;">
                <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px;">
                    <strong>${escapeHtml(cliente.nome)}</strong>
                    <span style="color: #f59e0b; font-weight: bold;">R$ ${cliente.totalDevido.toFixed(2)}</span>
                </div>
                <div style="font-size: 12px; color: #6b7280;">
                    ${cliente.vendas.length} venda(s) pendente(s)
                </div>
                <button onclick="verDetalhesCliente('${escapeHtml(cliente.nome)}')" style="margin-top: 10px; padding: 5px 10px; background: #2563eb; color: white; border: none; border-radius: 4px; cursor: pointer;">
                    Ver detalhes
                </button>
            </div>
        `;
    });
    
    container.innerHTML = html;
}

// Atualizar lista de vendas pendentes
function atualizarListaVendasPendentes(vendas) {
    const container = document.getElementById('listaVendasPendentes');
    if (!container) return;
    
    if (!vendas.length) {
        container.innerHTML = `
            <div class="centro-vazio">
                <div class="circulo-sucesso">✓</div>
                <p>Nenhuma venda fiada pendente</p>
            </div>
        `;
        return;
    }
    
    let html = '';
    vendas.forEach(venda => {
        const data = new Date(venda.dataHora);
        const dataFormatada = data.toLocaleDateString('pt-BR');
        
        html += `
            <div style="padding: 15px; border-bottom: 1px solid #e5e7eb;">
                <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px;">
                    <div>
                        <strong>${escapeHtml(venda.nomeCliente)}</strong><br>
                        <small style="color: #6b7280;">${dataFormatada}</small>
                    </div>
                    <span style="color: #f59e0b; font-weight: bold;">R$ ${venda.valorPendente.toFixed(2)}</span>
                </div>
                <div style="display: flex; gap: 10px; margin-top: 10px; flex-wrap: wrap;">
                    <input type="number" id="valorPagamento_${venda.idVenda}" placeholder="Valor do pagamento" step="0.01" style="flex: 1; padding: 8px; border: 1px solid #e5e7eb; border-radius: 4px;">
                    <button onclick="registrarPagamento(${venda.idVenda}, '${escapeHtml(venda.nomeCliente)}', ${venda.valorPendente})" style="padding: 8px 15px; background: #10b981; color: white; border: none; border-radius: 4px; cursor: pointer;">
                        Registrar Pagamento
                    </button>
                    <button onclick="gerarComprovanteVenda(${venda.idVenda})" style="padding: 8px 15px; background: #2563eb; color: white; border: none; border-radius: 4px; cursor: pointer;">
                        <i class="fa-solid fa-print"></i> Comprovante
                    </button>
                </div>
            </div>
        `;
    });
    
    container.innerHTML = html;
}

// Registrar pagamento
async function registrarPagamento(idVenda, nomeCliente, valorTotal) {
    const inputValor = document.getElementById(`valorPagamento_${idVenda}`);
    const valorPago = parseFloat(inputValor?.value || 0);
    const saldoAnterior = valorTotal;
    
    if (!valorPago || valorPago <= 0) {
        alert('Informe um valor válido para o pagamento!');
        return;
    }
    
    if (valorPago > valorTotal) {
        alert(`Valor pago (R$ ${valorPago.toFixed(2)}) excede o débito (R$ ${valorTotal.toFixed(2)})`);
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE_URL}/Fiado/pagar`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                idVenda: idVenda,
                valorPago: valorPago
            })
        });
        
        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.mensagem || 'Erro ao registrar pagamento');
        }
        
        const result = await response.json();
        alert(result.mensagem);
        
        // Gerar comprovante de pagamento
        await gerarComprovantePagamento(idVenda, valorPago, saldoAnterior);
        
        // Recarregar dados
        carregarFiados();
        
    } catch (error) {
        console.error('Erro ao registrar pagamento:', error);
        alert('Erro ao registrar pagamento: ' + error.message);
    }
}

function mostrarErroFiados() {
    const container = document.getElementById('listaClientesDebito');
    if (container) {
        container.innerHTML = `
            <div class="centro-vazio" style="color: #ef4444;">
                <p>Erro ao carregar dados</p>
                <p style="font-size: 12px;">Verifique se a API está rodando em ${API_BASE_URL}</p>
            </div>
        `;
    }
}

function verDetalhesCliente(nomeCliente) {
    alert(`Detalhes do cliente ${nomeCliente} - Funcionalidade em desenvolvimento`);
}

// ==================== COMPROVANTES ====================

// Gerar comprovante de venda
async function gerarComprovanteVenda(idVenda) {
    try {
        const response = await fetch(`${API_BASE_URL}/Comprovante/venda/${idVenda}`);
        
        if (!response.ok) {
            throw new Error('Erro ao gerar comprovante');
        }
        
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `comprovante_venda_${idVenda}.pdf`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        
        alert('Comprovante gerado com sucesso!');
    } catch (error) {
        console.error('Erro ao gerar comprovante:', error);
        alert('Erro ao gerar comprovante: ' + error.message);
    }
}

// Gerar comprovante de pagamento (para ser chamado após registrar pagamento)
async function gerarComprovantePagamento(idVenda, valorPago, saldoAnterior) {
    try {
        const response = await fetch(`${API_BASE_URL}/Comprovante/pagamento/${idVenda}?valorPago=${valorPago}&saldoAnterior=${saldoAnterior}`);
        
        if (!response.ok) {
            throw new Error('Erro ao gerar comprovante');
        }
        
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `recibo_pagamento_venda_${idVenda}.pdf`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
        
        alert('Recibo gerado com sucesso!');
    } catch (error) {
        console.error('Erro ao gerar recibo:', error);
        alert('Erro ao gerar recibo: ' + error.message);
    }
}

// ==================== HISTÓRICO DE FIADO ====================

// Abrir modal com histórico do cliente
async function verDetalhesCliente(nomeCliente) {
    // Primeiro, precisamos buscar o ID do cliente pelo nome
    try {
        const response = await fetch(`${API_BASE_URL}/Cliente`);
        const clientes = await response.json();
        
        const cliente = clientes.find(c => c.nome === nomeCliente);
        
        if (!cliente) {
            alert('Cliente não encontrado');
            return;
        }
        
        await carregarHistoricoCliente(cliente.idPessoa, cliente.nome);
        
    } catch (error) {
        console.error('Erro ao buscar cliente:', error);
        alert('Erro ao carregar dados do cliente');
    }
}

// Carregar histórico do cliente
async function carregarHistoricoCliente(clienteId, nomeCliente) {
    try {
        const response = await fetch(`${API_BASE_URL}/Fiado/historico/${clienteId}`);
        
        if (!response.ok) {
            throw new Error('Erro ao carregar histórico');
        }
        
        const dados = await response.json();
        
        // Atualizar modal
        document.getElementById('historicoClienteNome').textContent = `Histórico - ${nomeCliente}`;
        document.getElementById('totalGeral').textContent = `R$ ${dados.totalGeral.toFixed(2)}`;
        document.getElementById('totalPago').textContent = `R$ ${dados.totalPago.toFixed(2)}`;
        document.getElementById('saldoAtual').textContent = `R$ ${dados.saldoAtual.toFixed(2)}`;
        
        // Montar tabela de transações
        const tbody = document.getElementById('historicoTransacoes');
        
        if (!dados.transacoes || dados.transacoes.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="5" style="text-align: center; padding: 40px;">
                        Nenhuma transação encontrada
                    </td>
                </tr>
            `;
        } else {
            let html = '';
            dados.transacoes.forEach(transacao => {
                const data = new Date(transacao.data);
                const dataFormatada = data.toLocaleDateString('pt-BR');
                const horaFormatada = data.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });
                
                const tipoClass = transacao.tipo === 'DÉBITO' ? 'texto-vermelho' : 'texto-verde';
                
                const valorFormatado = transacao.tipo === 'DÉBITO' 
                    ? `- R$ ${transacao.valor.toFixed(2)}`
                    : `+ R$ ${transacao.valor.toFixed(2)}`;
                const valorColor = transacao.tipo === 'DÉBITO' ? '#ef4444' : '#10b981';
                
                html += `
                    <tr style="border-bottom: 1px solid #e5e7eb;">
                        <td style="padding: 12px;">${dataFormatada} ${horaFormatada}</td>
                        <td style="padding: 12px;">
                            <span style="background: ${transacao.tipo === 'DÉBITO' ? '#fee2e2' : '#dcfce7'}; padding: 4px 8px; border-radius: 4px; font-size: 12px;">
                                ${transacao.tipo}
                            </span>
                        </td>
                        <td style="padding: 12px;">${escapeHtml(transacao.descricao)}</td>
                        <td style="padding: 12px; text-align: right; color: ${valorColor}; font-weight: bold;">
                            ${valorFormatado}
                        </td>
                        <td style="padding: 12px; text-align: right; font-weight: bold;">
                            R$ ${transacao.saldoApos.toFixed(2)}
                        </td>
                    </tr>
                `;
            });
            tbody.innerHTML = html;
        }
        
        // Abrir modal
        const modal = document.getElementById('modalHistorico');
        if (modal) {
            modal.style.display = 'block';
        }
        
    } catch (error) {
        console.error('Erro ao carregar histórico:', error);
        alert('Erro ao carregar histórico: ' + error.message);
    }
}

// Fechar modal de histórico
function fecharModalHistorico() {
    const modal = document.getElementById('modalHistorico');
    if (modal) {
        modal.style.display = 'none';
    }
}

// ==================== RELATÓRIO DO DIA ====================

// Gerar relatório do dia
async function gerarRelatorioDia() {
    try {
        const response = await fetch(`${API_BASE_URL}/Dashboard`);
        
        if (!response.ok) {
            throw new Error('Erro ao carregar dados');
        }
        
        const dados = await response.json();
        
        // Buscar total de fiados acumulados
        const responseFiado = await fetch(`${API_BASE_URL}/Fiado/resumo`);
        const dadosFiado = await responseFiado.json();
        
        // Usar os novos campos
        const vendasVista = dados.vendasVistaHoje || 0;
        const vendasFiadasNovas = dados.vendasFiadasHoje || 0;
        const totalGeral = dados.vendasHoje.totalValor || 0;
        const totalReceber = dadosFiado.totalAReceber || 0;
        
        // Atualizar modal
        const hoje = new Date();
        const dataFormatada = hoje.toLocaleDateString('pt-BR', { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' });
        
        document.getElementById('dataRelatorio').textContent = dataFormatada;
        document.getElementById('vendasVista').textContent = `R$ ${vendasVista.toFixed(2)}`;
        document.getElementById('vendasFiadas').textContent = `R$ ${vendasFiadasNovas.toFixed(2)}`;
        document.getElementById('totalGeralDia').textContent = `R$ ${totalGeral.toFixed(2)}`;
        document.getElementById('totalReceber').textContent = `R$ ${totalReceber.toFixed(2)}`;
        
        const modal = document.getElementById('modalRelatorio');
        if (modal) {
            modal.style.display = 'block';
        }
        
    } catch (error) {
        console.error('Erro ao gerar relatório:', error);
        alert('Erro ao gerar relatório: ' + error.message);
    }
}

// Fechar modal de relatório
function fecharModalRelatorio() {
    const modal = document.getElementById('modalRelatorio');
    if (modal) {
        modal.style.display = 'none';
    }
}

// Imprimir relatório
function imprimirRelatorio() {
    const conteudo = document.getElementById('modalRelatorio').innerHTML;
    const janela = window.open('', '_blank');
    janela.document.write(`
        <html>
            <head>
                <title>Relatório do Dia - Comercial Morro</title>
                <style>
                    body { font-family: Arial, sans-serif; padding: 20px; }
                    h2 { color: #1e3a8a; }
                    .card { margin-bottom: 20px; padding: 15px; border: 1px solid #ddd; border-radius: 8px; }
                    .row { display: flex; justify-content: space-between; margin-bottom: 10px; }
                    .total { font-size: 18px; font-weight: bold; color: #2563eb; }
                    .footer { margin-top: 30px; text-align: center; color: #6b7280; font-size: 12px; }
                </style>
            </head>
            <body>
                <h2>COMERCIAL MORRO</h2>
                <h3>Relatório do Dia</h3>
                ${document.getElementById('dataRelatorio').innerHTML ? `<p>${document.getElementById('dataRelatorio').textContent}</p>` : ''}
                <div class="card">
                    <div class="row"><strong>Vendas à Vista:</strong> ${document.getElementById('vendasVista').textContent}</div>
                    <div class="row"><strong>Vendas Fiadas (novas):</strong> ${document.getElementById('vendasFiadas').textContent}</div>
                    <div class="row total"><strong>Total Geral:</strong> ${document.getElementById('totalGeralDia').textContent}</div>
                </div>
                <div class="card">
                    <div class="row"><strong>Total a Receber (Fiados acumulados):</strong> ${document.getElementById('totalReceber').textContent}</div>
                </div>
                <div class="footer">
                    Gerado em ${new Date().toLocaleString('pt-BR')}
                </div>
            </body>
        </html>
    `);
    janela.document.close();
    janela.print();
}

// ==================== INICIALIZAÇÃO ÚNICA ====================
document.addEventListener('DOMContentLoaded', () => {
    // Verificar autenticação (exceto na página de login)
    const paginaAtual = window.location.pathname.split("/").pop();
    
    if (paginaAtual !== 'login.html') {
        if (!protegerRota()) return;
    }
    
    // Ativar o menu lateral conforme a página atual
    ativarMenuAtual();
    
    // Adicionar botão de logout no menu (se existir)
    adicionarBotaoLogout();
    
    // ========== PÁGINA INICIAL (DASHBOARD) ==========
    if (paginaAtual === 'index.html' || paginaAtual === '' || paginaAtual === '/') {
        carregarDashboard();
    }
    
    // ========== PÁGINA DE CLIENTES ==========
    if (paginaAtual === 'clientes.html') {
        const campoBusca = document.querySelector('.campo-busca-clientes');
        if (campoBusca) {
            campoBusca.addEventListener('input', (e) => {
                buscarClientes(e.target.value);
            });
        }
        carregarClientes();
        atualizarEstatisticasClientes();
    }
    
    // ========== PÁGINA DE PRODUTOS ==========
    if (paginaAtual === 'produtos.html') {
        carregarProdutos();
        carregarCategorias();
    }
    
    // ========== PÁGINA DE ESTOQUE ==========
    if (paginaAtual === 'estoque.html') {
        carregarResumoEstoque();
        carregarTabelaEstoque();
        carregarFiltrosCategoria();
        
        const inputBusca = document.getElementById('buscaProduto');
        if (inputBusca) {
            inputBusca.addEventListener('input', debounce(aplicarBusca, 500));
        }
    }
    
    // ========== PÁGINA DE VENDAS ==========
    if (paginaAtual === 'novaVenda.html') {
        carregarProdutosVenda();
        carregarClientesVenda();
        
        const formaPagamento = document.getElementById('formaPagamento');
        if (formaPagamento) {
            formaPagamento.addEventListener('change', toggleClienteFiado);
        }
        
        const buscaProduto = document.getElementById('buscarProduto');
        if (buscaProduto) {
            buscaProduto.addEventListener('input', buscarProdutosVenda);
        }
    }
    
    // ========== PÁGINA DE FIADOS ==========
    if (paginaAtual === 'fiado.html') {
        carregarFiados();
    }
});

// Adicionar botão de logout no menu
function adicionarBotaoLogout() {
    const username = localStorage.getItem('username');
    const tipo = localStorage.getItem('userTipo');
    
    if (!username) return;
    
    const menuLista = document.querySelector('.lista-menu');
    if (!menuLista) return;
    
    // Verificar se já existe o item de logout
    if (document.getElementById('menuLogout')) return;
    
    // Criar item de logout
    const li = document.createElement('li');
    li.id = 'menuLogout';
    li.className = 'item-menu';
    li.style.marginTop = '40px';
    li.style.borderTop = '1px solid rgba(255,255,255,0.2)';
    li.style.paddingTop = '15px';
    li.innerHTML = `
        <i class="fa-solid fa-sign-out-alt"></i> Sair (${username} - ${tipo})
    `;
    li.onclick = () => logout();
    
    menuLista.appendChild(li);
}

// Função de debounce para busca
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}
//PIM 3.0 SEMESTRE