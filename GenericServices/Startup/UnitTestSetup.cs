﻿// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using GenericServices.Configuration;
using GenericServices.Startup.Internal;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using GenericServices.Configuration.Internal;
using GenericServices.Internal.Decoders;

namespace GenericServices.Startup
{
    public static class UnitTestSetup
    {
        /// <summary>
        /// This is designed to set up the system for using one DTO in a unit test of a service
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="context"></param>
        /// <param name="publicConfig"></param>
        /// <returns></returns>
        public static IWrappedAutoMapperConfig SetupSingleDtoAndEntities<TDto>(this DbContext context,
            IGenericServicesConfig publicConfig = null)
        {
            var status = new StatusGenericHandler();
            publicConfig = publicConfig ?? new GenericServicesConfig();
            context.RegisterEntityClasses();
            var typesInAssembly = typeof(TDto).Assembly.GetTypes();
            var dtoRegister = new RegisterOneDtoType(typeof(TDto), typesInAssembly, publicConfig);
            status.CombineStatuses(dtoRegister);
            if (!status.IsValid)
                throw new InvalidOperationException($"SETUP FAILED with {status.Errors.Count} errors. Errors are:\n" 
                                                    + status.GetAllErrors());

            var readProfile = new MappingProfile(false);
            var saveProfile = new MappingProfile(true);
            SetupDtosAndMappings.SetupMappingForDto(dtoRegister, readProfile, saveProfile);
            return SetupDtosAndMappings.CreateWrappedAutoMapperConfig(readProfile, saveProfile);
        }
    }
}