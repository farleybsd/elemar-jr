public readonly record struct PedidoResponse(
    Guid Id,
    string Cliente,
    string Endereco,
    string Telefone,
    string Email,
    decimal ValorTotal,
    string Observacoes,
    List<ItemPedidoResponse> Itens
);

public readonly record struct ItemPedidoResponse(
    Guid ProdutoId,
    string NomeProduto,
    decimal Preco,
    int Quantidade
);

// REQUEST 
public readonly record struct CriarProdutoRequest(
    [property: JsonPropertyName("nome")] string Nome,
    [property: JsonPropertyName("preco")] decimal Preco,
    [property: JsonPropertyName("estoqueInicial")] int EstoqueInicial
);

// RESPONSE 
public readonly record struct CriarProdutoResponse(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("nome")] string Nome,
    [property: JsonPropertyName("preco")] decimal Preco,
    [property: JsonPropertyName("estoque")] int Estoque,
    [property: JsonPropertyName("sucesso")] bool Sucesso,
    [property: JsonPropertyName("mensagem")] string Mensagem
);