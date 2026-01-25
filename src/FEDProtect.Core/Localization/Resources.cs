namespace FEDProtect.Core.Localization;

public static class Resources
{
    private static readonly Dictionary<string, Dictionary<string, string>> _translations = new()
    {
        ["en"] = new Dictionary<string, string>
        {
            ["app.title"] = "FEDProtect",
            ["app.subtitle"] = "Translate surveillance threats into device hardening",
            ["profile.list.title"] = "Threat Profiles",
            ["profile.refresh"] = "Refresh Profiles",
            ["profile.none_found"] = "No profiles found. Add YAML profiles to the profiles/ directory.",
            ["action.generate_checklist"] = "Generate Checklist",
            ["action.create_firefox_profile"] = "Create Firefox Profile",
            ["action.run_security_checks"] = "Run Security Checks",
            ["action.apply"] = "Apply",
            ["tab.threats"] = "Threats",
            ["tab.security_checks"] = "Security Checks",
            ["tab.about"] = "About",
            ["mitigations.title"] = "Mitigations",
            ["mitigations.select_threat"] = "Select a threat to view mitigations",
            ["security.run_checks_help"] = "Run security checks to verify your system configuration",
            ["status.ready"] = "Ready",
            ["status.loading_profiles"] = "Loading profiles...",
            ["status.loading"] = "Loading {0}...",
            ["status.loaded"] = "Loaded: {0}",
            ["status.generating_checklist"] = "Generating checklist...",
            ["status.checklist_saved"] = "Checklist saved to: {0}",
            ["status.creating_firefox_profile"] = "Creating Firefox profile...",
            ["status.firefox_profile_saved"] = "Firefox profile saved to: {0}",
            ["status.running_checks"] = "Running security checks...",
            ["status.checks_complete"] = "Checks complete: {0}/{1} passed",
            ["status.save_cancelled"] = "Save cancelled",
            ["status.select_profile_first"] = "Please select a profile first",
            ["error.prefix"] = "Error: {0}",
            ["error.file_not_found"] = "File not found: {0}",
            ["error.parse_failed"] = "Failed to parse YAML",
            ["validation.passed"] = "Validation PASSED",
            ["validation.failed"] = "Validation FAILED",
            ["validation.errors"] = "Errors:",
            ["validation.warnings"] = "Warnings:",
            ["validation.profile_name_required"] = "Profile must have a 'name' field",
            ["validation.profile_description_recommended"] = "Profile should have a 'description' field",
            ["validation.profile_version_recommended"] = "Profile should have a 'version' field",
            ["validation.capabilities_required"] = "Profile must have at least one capability",
            ["validation.capability_id_required"] = "Capability is missing 'id' field",
            ["validation.capability_name_required"] = "Capability '{0}' is missing 'name' field",
            ["validation.capability_sources_recommended"] = "Capability '{0}' has no sources cited",
            ["validation.mitigations_required"] = "Capability '{0}' must have at least one mitigation",
            ["about.description"] = "FEDProtect translates real-time federal surveillance threats into automated device hardening. Community-maintained YAML threat profiles feed into actionable security configurations.",
            ["about.resources"] = "Resources",
            ["about.license"] = "License",
            ["about.license_text"] = "GPLv3 Open Source",
            ["about.disclaimer"] = "Disclaimer",
            ["about.disclaimer_text"] = "This tool does not guarantee protection. Threat landscape evolves constantly. This is not legal advice. Consult with lawyers and security professionals.",
            ["legal.do_not_unlock"] = "Do NOT unlock your device if arrested; invoke right to remain silent.",
            ["legal.invoke_rights"] = "Say: \"I am invoking my right to remain silent. I want a lawyer.\"",
            ["legal.memorize_lawyer"] = "Memorize a lawyer's phone number; call immediately upon arrest."
        },
        ["es"] = new Dictionary<string, string>
        {
            ["app.title"] = "FEDProtect",
            ["app.subtitle"] = "Traduce amenazas de vigilancia en protección de dispositivos",
            ["profile.list.title"] = "Perfiles de Amenazas",
            ["profile.refresh"] = "Actualizar Perfiles",
            ["profile.none_found"] = "No se encontraron perfiles. Añade archivos YAML al directorio profiles/.",
            ["action.generate_checklist"] = "Generar Lista de Verificación",
            ["action.create_firefox_profile"] = "Crear Perfil de Firefox",
            ["action.run_security_checks"] = "Ejecutar Verificaciones de Seguridad",
            ["action.apply"] = "Aplicar",
            ["tab.threats"] = "Amenazas",
            ["tab.security_checks"] = "Verificaciones de Seguridad",
            ["tab.about"] = "Acerca de",
            ["mitigations.title"] = "Mitigaciones",
            ["mitigations.select_threat"] = "Selecciona una amenaza para ver mitigaciones",
            ["status.ready"] = "Listo",
            ["status.loading_profiles"] = "Cargando perfiles...",
            ["status.select_profile_first"] = "Por favor selecciona un perfil primero",
            ["legal.do_not_unlock"] = "NO desbloquees tu dispositivo si eres arrestado; invoca tu derecho a permanecer en silencio.",
            ["legal.memorize_lawyer"] = "Memoriza el número de un abogado; llama inmediatamente al ser arrestado."
        }
    };

    private static string _currentLanguage = "en";

    public static string CurrentLanguage
    {
        get => _currentLanguage;
        set => _currentLanguage = _translations.ContainsKey(value) ? value : "en";
    }

    public static IEnumerable<string> AvailableLanguages => _translations.Keys;

    public static string Get(string key)
    {
        if (_translations.TryGetValue(_currentLanguage, out var langDict) && 
            langDict.TryGetValue(key, out var value))
        {
            return value;
        }

        if (_translations.TryGetValue("en", out var defaultDict) && 
            defaultDict.TryGetValue(key, out var defaultValue))
        {
            return defaultValue;
        }

        return $"[{key}]";
    }

    public static string Get(string key, params object[] args)
    {
        var template = Get(key);
        try
        {
            return string.Format(template, args);
        }
        catch
        {
            return template;
        }
    }
}
