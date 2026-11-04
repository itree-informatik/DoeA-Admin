// SPDX-License-Identifier: AGPL-3.0-or-later
// Copyright (C) 2026 Schweizerische Eidgenossenschaft, Bundesamt für Bauten und Logistik BBL

namespace DoeA.Interfaces;

/// <summary>Mapping zwischen DB-Entity und DTO eines Slices, generiert durch Mapperly.</summary>
public interface IEntityMapper<TModel, TEntity>
{
    /// <summary>Erzeugt aus der Entity ein neues DTO.</summary>
    TModel ZuModel(TEntity entity);

    /// <summary>Erzeugt aus dem DTO eine neue Entity; Navigationen bleiben leer.</summary>
    TEntity ZuEntity(TModel model);
}
