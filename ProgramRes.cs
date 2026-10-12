using System;
using System.Globalization;
using System.Text;

namespace ProjetoRestaurante
{
    // ---------------------------------------------------------------
    // Item
    // ---------------------------------------------------------------
    public class Item
    {
        private int id;
        private string descricao;
        private double preco;

        public Item(int id, string descricao, double preco)
        {
            this.id = id;
            this.descricao = descricao;
            this.preco = preco;
        }

        public int Id => id;
        public string Descricao => descricao;
        public double Preco => preco;
    }

    // ---------------------------------------------------------------
    // Pedido
    // ---------------------------------------------------------------
    public class Pedido
    {
        private const int MaxItens = 10;

        private int id;
        private string cliente;
        private Item[] itens = new Item[MaxItens];

        public Pedido(string cliente)
        {
            this.cliente = cliente;
        }

        // Usado pelo Restaurante para numerar o pedido sequencialmente
        public int Id
        {
            get => id;
            set => id = value;
        }

        public string Cliente => cliente;

        // Retorna false se o pedido já tem 10 itens
        public bool adicionarItem(Item item)
        {
            if (item == null) return false;

            for (int i = 0; i < itens.Length; i++)
            {
                if (itens[i] == null)
                {
                    itens[i] = item;
                    return true;
                }
            }
            return false;
        }

        // Remove a primeira ocorrência do item (comparando pelo id)
        // e compacta o vetor para não deixar buracos.
        public bool removerItem(Item item)
        {
            if (item == null) return false;

            for (int i = 0; i < itens.Length; i++)
            {
                if (itens[i] != null && itens[i].Id == item.Id)
                {
                    for (int j = i; j < itens.Length - 1; j++)
                        itens[j] = itens[j + 1];
                    itens[itens.Length - 1] = null;
                    return true;
                }
            }
            return false;
        }

        public double calcularTotal()
        {
            double total = 0;
            foreach (Item item in itens)
            {
                if (item != null) total += item.Preco;
            }
            return total;
        }

        public string dadosDoPedido()
        {
            CultureInfo br = new CultureInfo("pt-BR");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Pedido #{id}");
            sb.AppendLine($"Cliente: {cliente}");
            sb.AppendLine("Itens:");

            bool temItem = false;
            foreach (Item item in itens)
            {
                if (item != null)
                {
                    temItem = true;
                    sb.AppendLine($"  - [{item.Id}] {item.Descricao} - {item.Preco.ToString("C2", br)}");
                }
            }
            if (!temItem) sb.AppendLine("  (nenhum item)");

            sb.Append($"Total: {calcularTotal().ToString("C2", br)}");
            return sb.ToString();
        }
    }

    // ---------------------------------------------------------------
    // Restaurante
    // ---------------------------------------------------------------
    public class Restaurante
    {
        private const int MaxPedidos = 50;

        private int proxPedido = 1;
        private Pedido[] pedidos = new Pedido[MaxPedidos];

        // Registra o pedido em uma posição livre e atribui o próximo número.
        // Retorna false se o limite de 50 pedidos do dia foi atingido.
        public bool novoPedido(Pedido pedido)
        {
            if (pedido == null) return false;

            for (int i = 0; i < pedidos.Length; i++)
            {
                if (pedidos[i] == null)
                {
                    pedido.Id = proxPedido++;
                    pedidos[i] = pedido;
                    return true;
                }
            }
            return false;
        }

        // Busca pelo id do pedido informado; retorna null se não existir
        public Pedido buscarPedido(Pedido pedido)
        {
            if (pedido == null) return null;
            return buscarPedido(pedido.Id);
        }

        // Sobrecarga de conveniência: o menu só tem o número do pedido
        public Pedido buscarPedido(int id)
        {
            foreach (Pedido p in pedidos)
            {
                if (p != null && p.Id == id) return p;
            }
            return null;
        }

        public bool cancelarPedido(Pedido pedido)
        {
            if (pedido == null) return false;

            for (int i = 0; i < pedidos.Length; i++)
            {
                if (pedidos[i] != null && pedidos[i].Id == pedido.Id)
                {
                    pedidos[i] = null;
                    return true;
                }
            }
            return false;
        }

        public Pedido[] ObterPedidos() => pedidos;
    }

    // ---------------------------------------------------------------
    // Programa principal (seletor)
    // ---------------------------------------------------------------
    class Program
    {
        static readonly CultureInfo Br = new CultureInfo("pt-BR");

        static readonly Item[] Cardapio =
        {
            new Item(1, "Prato executivo", 28.90),
            new Item(2, "Feijoada", 34.50),
            new Item(3, "Lasanha", 31.00),
            new Item(4, "Salada Caesar", 22.00),
            new Item(5, "Suco natural", 9.00),
            new Item(6, "Refrigerante", 7.00),
            new Item(7, "Sobremesa do dia", 12.50),
        };

        static void Main()
        {
            Restaurante restaurante = new Restaurante();
            int opcao;

            do
            {
                Console.WriteLine();
                Console.WriteLine("===== RESTAURANTE =====");
                Console.WriteLine("0. Sair");
                Console.WriteLine("1. Criar novo pedido");
                Console.WriteLine("2. Adicionar item ao pedido");
                Console.WriteLine("3. Remover item do pedido");
                Console.WriteLine("4. Consultar pedido");
                Console.WriteLine("5. Cancelar pedido");
                Console.WriteLine("6. Listar todos os pedidos");
                opcao = LerInteiro("Opção: ");

                switch (opcao)
                {
                    case 0: Console.WriteLine("Encerrando..."); break;
                    case 1: CriarPedido(restaurante); break;
                    case 2: AdicionarItem(restaurante); break;
                    case 3: RemoverItem(restaurante); break;
                    case 4: ConsultarPedido(restaurante); break;
                    case 5: CancelarPedido(restaurante); break;
                    case 6: ListarPedidos(restaurante); break;
                    default: Console.WriteLine("Opção inválida."); break;
                }
            } while (opcao != 0);
        }

        static void CriarPedido(Restaurante restaurante)
        {
            Console.Write("Nome do cliente: ");
            string cliente = (Console.ReadLine() ?? "").Trim();
            if (cliente == "")
            {
                Console.WriteLine("O nome do cliente é obrigatório.");
                return;
            }

            Pedido pedido = new Pedido(cliente);
            if (restaurante.novoPedido(pedido))
                Console.WriteLine($"Pedido #{pedido.Id} criado para {cliente}.");
            else
                Console.WriteLine("Limite de 50 pedidos do dia atingido.");
        }

        static void AdicionarItem(Restaurante restaurante)
        {
            Pedido pedido = SelecionarPedido(restaurante);
            if (pedido == null) return;

            Item item = SelecionarItemDoCardapio();
            if (item == null) return;

            if (pedido.adicionarItem(item))
                Console.WriteLine($"'{item.Descricao}' adicionado ao pedido #{pedido.Id}.");
            else
                Console.WriteLine("O pedido já possui o máximo de 10 itens.");
        }

        static void RemoverItem(Restaurante restaurante)
        {
            Pedido pedido = SelecionarPedido(restaurante);
            if (pedido == null) return;

            Item item = SelecionarItemDoCardapio();
            if (item == null) return;

            if (pedido.removerItem(item))
                Console.WriteLine($"'{item.Descricao}' removido do pedido #{pedido.Id}.");
            else
                Console.WriteLine("Esse item não está no pedido.");
        }

        static void ConsultarPedido(Restaurante restaurante)
        {
            Pedido pedido = SelecionarPedido(restaurante);
            if (pedido == null) return;

            Console.WriteLine();
            Console.WriteLine(pedido.dadosDoPedido());
        }

        static void CancelarPedido(Restaurante restaurante)
        {
            Pedido pedido = SelecionarPedido(restaurante);
            if (pedido == null) return;

            if (restaurante.cancelarPedido(pedido))
                Console.WriteLine($"Pedido #{pedido.Id} cancelado.");
            else
                Console.WriteLine("Não foi possível cancelar o pedido.");
        }

        static void ListarPedidos(Restaurante restaurante)
        {
            double somaGeral = 0;
            int quantidade = 0;

            Console.WriteLine();
            Console.WriteLine("----- Pedidos do dia -----");
            foreach (Pedido p in restaurante.ObterPedidos())
            {
                if (p == null) continue;

                double total = p.calcularTotal();
                Console.WriteLine($"Pedido #{p.Id} | {p.Cliente} | {total.ToString("C2", Br)}");
                somaGeral += total;
                quantidade++;
            }

            if (quantidade == 0)
                Console.WriteLine("Nenhum pedido registrado.");
            else
                Console.WriteLine($"Soma geral do dia ({quantidade} pedidos): {somaGeral.ToString("C2", Br)}");
        }

        // ----- Auxiliares -----

        static Pedido SelecionarPedido(Restaurante restaurante)
        {
            int id = LerInteiro("Número do pedido: ");
            Pedido pedido = restaurante.buscarPedido(id);
            if (pedido == null)
                Console.WriteLine("Pedido não encontrado.");
            return pedido;
        }

        static Item SelecionarItemDoCardapio()
        {
            Console.WriteLine("Cardápio:");
            foreach (Item i in Cardapio)
                Console.WriteLine($"  {i.Id}. {i.Descricao} - {i.Preco.ToString("C2", Br)}");

            int id = LerInteiro("Código do item: ");
            foreach (Item i in Cardapio)
            {
                if (i.Id == id) return i;
            }
            Console.WriteLine("Item não encontrado no cardápio.");
            return null;
        }

        static int LerInteiro(string mensagem)
        {
            int valor;
            Console.Write(mensagem);
            while (!int.TryParse(Console.ReadLine(), out valor))
            {
                Console.Write("Valor inválido. " + mensagem);
            }
            return valor;
        }
    }
}
