using System.Security;

namespace ClangSharp
{
	using System;
	using System.Runtime.InteropServices;

	public partial struct CXString
	{
		public IntPtr @data;
		public uint @private_flags;
	}

	public partial struct CXVirtualFileOverlayImpl
	{
	}

	public partial struct CXModuleMapDescriptorImpl
	{
	}

	public partial struct CXTranslationUnitImpl
	{
	}

	public partial struct CXUnsavedFile
	{
		[MarshalAs(UnmanagedType.LPStr)] public string @Filename;
		[MarshalAs(UnmanagedType.LPStr)] public string @Contents;
		public int @Length;
	}

	public partial struct CXVersion
	{
		public int @Major;
		public int @Minor;
		public int @Subminor;
	}

	public partial struct CXFileUniqueID
	{
		public ulong @data0; public ulong @data1; public ulong @data2; 
	}

	public partial struct CXSourceLocation
	{
		public IntPtr @ptr_data0; public IntPtr @ptr_data1; 
		public uint @int_data;
	}

	public partial struct CXSourceRange
	{
		public IntPtr @ptr_data0; public IntPtr @ptr_data1; 
		public uint @begin_int_data;
		public uint @end_int_data;
	}

	public partial struct CXSourceRangeList
	{
		public uint @count;
		public IntPtr @ranges;
	}

	public partial struct CXTUResourceUsageEntry
	{
		public CXTUResourceUsageKind @kind;
		public int @amount;
	}

	public partial struct CXTUResourceUsage
	{
		public IntPtr @data;
		public uint @numEntries;
		public IntPtr @entries;
	}

	public partial struct CXCursor
	{
		public CXCursorKind @kind;
		public int @xdata;
		public IntPtr @data0; public IntPtr @data1; public IntPtr @data2; 
	}

	public partial struct CXPlatformAvailability
	{
		public CXString @Platform;
		public CXVersion @Introduced;
		public CXVersion @Deprecated;
		public CXVersion @Obsoleted;
		public int @Unavailable;
		public CXString @Message;
	}

	public partial struct CXCursorSetImpl
	{
	}

	public partial struct CXType
	{
		public CXTypeKind @kind;
		public IntPtr @data0; public IntPtr @data1; 
	}

	public partial struct CXToken
	{
		public uint @int_data0; public uint @int_data1; public uint @int_data2; public uint @int_data3; 
		public IntPtr @ptr_data;
	}

	public partial struct CXCompletionResult
	{
		public CXCursorKind @CursorKind;
		public IntPtr @CompletionString;
	}

	[StructLayout (LayoutKind.Sequential)]
	public partial struct CXCodeCompleteResults
	{
		public IntPtr @Results;
		public uint @NumResults;
	}

	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate CXVisitorResult CXCursorAndRangeVisitorDelegate(IntPtr @context, CXCursor @cursor, CXSourceRange @range);

	public partial struct CXCursorAndRangeVisitor
	{
		public IntPtr @context;

		public CXCursorAndRangeVisitorDelegate visit;
	}

	public partial struct CXIdxLoc
	{
		public IntPtr @ptr_data0; public IntPtr @ptr_data1; 
		public uint @int_data;
	}

	public partial struct CXIdxIncludedFileInfo
	{
		public CXIdxLoc @hashLoc;
		[MarshalAs(UnmanagedType.LPStr)] public string @filename;
		public IntPtr @file;
		public int @isImport;
		public int @isAngled;
		public int @isModuleImport;
	}

	public partial struct CXIdxImportedASTFileInfo
	{
		public IntPtr @file;
		public IntPtr @module;
		public CXIdxLoc @loc;
		public int @isImplicit;
	}

	public partial struct CXIdxAttrInfo
	{
		public CXIdxAttrKind @kind;
		public CXCursor @cursor;
		public CXIdxLoc @loc;
	}

	public partial struct CXIdxEntityInfo
	{
		public CXIdxEntityKind @kind;
		public CXIdxEntityCXXTemplateKind @templateKind;
		public CXIdxEntityLanguage @lang;
		[MarshalAs(UnmanagedType.LPStr)] public string @name;
		[MarshalAs(UnmanagedType.LPStr)] public string @USR;
		public CXCursor @cursor;
		public IntPtr @attributes;
		public uint @numAttributes;
	}

	public partial struct CXIdxContainerInfo
	{
		public CXCursor @cursor;
	}

	public partial struct CXIdxIBOutletCollectionAttrInfo
	{
		public IntPtr @attrInfo;
		public IntPtr @objcClass;
		public CXCursor @classCursor;
		public CXIdxLoc @classLoc;
	}

	public partial struct CXIdxDeclInfo
	{
		public IntPtr @entityInfo;
		public CXCursor @cursor;
		public CXIdxLoc @loc;
		public IntPtr @semanticContainer;
		public IntPtr @lexicalContainer;
		public int @isRedeclaration;
		public int @isDefinition;
		public int @isContainer;
		public IntPtr @declAsContainer;
		public int @isImplicit;
		public IntPtr @attributes;
		public uint @numAttributes;
		public uint @flags;
	}

	public partial struct CXIdxObjCContainerDeclInfo
	{
		public IntPtr @declInfo;
		public CXIdxObjCContainerKind @kind;
	}

	public partial struct CXIdxBaseClassInfo
	{
		public IntPtr @base;
		public CXCursor @cursor;
		public CXIdxLoc @loc;
	}

	public partial struct CXIdxObjCProtocolRefInfo
	{
		public IntPtr @protocol;
		public CXCursor @cursor;
		public CXIdxLoc @loc;
	}

	public partial struct CXIdxObjCProtocolRefListInfo
	{
		public IntPtr @protocols;
		public uint @numProtocols;
	}

	public partial struct CXIdxObjCInterfaceDeclInfo
	{
		public IntPtr @containerInfo;
		public IntPtr @superInfo;
		public IntPtr @protocols;
	}

	public partial struct CXIdxObjCCategoryDeclInfo
	{
		public IntPtr @containerInfo;
		public IntPtr @objcClass;
		public CXCursor @classCursor;
		public CXIdxLoc @classLoc;
		public IntPtr @protocols;
	}

	public partial struct CXIdxObjCPropertyDeclInfo
	{
		public IntPtr @declInfo;
		public IntPtr @getter;
		public IntPtr @setter;
	}

	public partial struct CXIdxCXXClassDeclInfo
	{
		public IntPtr @declInfo;
		public IntPtr @bases;
		public uint @numBases;
	}

	public partial struct CXIdxEntityRefInfo
	{
		public CXIdxEntityRefKind @kind;
		public CXCursor @cursor;
		public CXIdxLoc @loc;
		public IntPtr @referencedEntity;
		public IntPtr @parentEntity;
		public IntPtr @container;
	}

	public partial struct IndexerCallbacks
	{
		public IntPtr @abortQuery;
		public IntPtr @diagnostic;
		public IntPtr @enteredMainFile;
		public IntPtr @ppIncludedFile;
		public IntPtr @importedASTFile;
		public IntPtr @startedTranslationUnit;
		public IntPtr @indexDeclaration;
		public IntPtr @indexEntityReference;
	}

	public partial struct CXComment
	{
		public IntPtr @ASTNode;
		public IntPtr @TranslationUnit;
	}

	public partial struct CXVirtualFileOverlay
	{
		public CXVirtualFileOverlay(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	public partial struct CXModuleMapDescriptor
	{
		public CXModuleMapDescriptor(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	public partial struct CXIndex
	{
		public CXIndex(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	public partial struct CXTranslationUnit
	{
		public CXTranslationUnit(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	public partial struct CXClientData
	{
		public CXClientData(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	public partial struct CXFile
	{
		public CXFile(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	public partial struct CXDiagnostic
	{
		public CXDiagnostic(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	public partial struct CXDiagnosticSet
	{
		public CXDiagnosticSet(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	public partial struct CXCursorSet
	{
		public CXCursorSet(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	[SuppressUnmanagedCodeSecurity]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate CXChildVisitResult CXCursorVisitor(CXCursor @cursor, CXCursor @parent, IntPtr @client_data);

	public partial struct CXModule
	{
		public CXModule(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	[StructLayout (LayoutKind.Sequential)]
	public partial struct CXCompletionString
	{
		public CXCompletionString(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void CXInclusionVisitor(IntPtr @included_file, out CXSourceLocation @inclusion_stack, uint @include_len, IntPtr @client_data);

	public partial struct CXRemapping
	{
		public CXRemapping(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	public partial struct CXIdxClientFile
	{
		public CXIdxClientFile(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	public partial struct CXIdxClientEntity
	{
		public CXIdxClientEntity(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	public partial struct CXIdxClientContainer
	{
		public CXIdxClientContainer(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	public partial struct CXIdxClientASTFile
	{
		public CXIdxClientASTFile(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	public partial struct CXIndexAction
	{
		public CXIndexAction(IntPtr pointer)
		{
			this.Pointer = pointer;
		}

		public IntPtr Pointer;
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate CXVisitorResult CXFieldVisitor(CXCursor @C, IntPtr @client_data);

	public enum CXErrorCode : uint
	{
		@Success = 0,
		@Failure = 1,
		@Crashed = 2,
		@InvalidArguments = 3,
		@ASTReadError = 4,
	}

	public enum CXAvailabilityKind : uint
	{
		@Available = 0,
		@Deprecated = 1,
		@NotAvailable = 2,
		@NotAccessible = 3,
	}

	public enum CXGlobalOptFlags : uint
	{
		@None = 0,
		@ThreadBackgroundPriorityForIndexing = 1,
		@ThreadBackgroundPriorityForEditing = 2,
		@ThreadBackgroundPriorityForAll = 3,
	}

    public enum CXDiagnosticSeverity : uint
	{
		@Ignored = 0,
		@Note = 1,
		@Warning = 2,
		@Error = 3,
		@Fatal = 4,
	}

    public enum CXLoadDiag_Error : uint
	{
		@None = 0,
		@Unknown = 1,
		@CannotLoad = 2,
		@InvalidFile = 3,
	}

    public enum CXDiagnosticDisplayOptions : uint
	{
		@DisplaySourceLocation = 1,
		@DisplayColumn = 2,
		@DisplaySourceRanges = 4,
		@DisplayOption = 8,
		@DisplayCategoryId = 16,
		@DisplayCategoryName = 32,
	}

    public enum CXTranslationUnit_Flags : uint
	{
		@None = 0,
		@DetailedPreprocessingRecord = 1,
		@Incomplete = 2,
		@PrecompiledPreamble = 4,
		@CacheCompletionResults = 8,
		@ForSerialization = 16,
		@CXXChainedPCH = 32,
		@SkipFunctionBodies = 64,
		@IncludeBriefCommentsInCodeCompletion = 128,
	}

	public enum CXSaveTranslationUnit_Flags : uint
	{
		@CXSaveTranslationUnit_None = 0,
	}

	public enum CXSaveError : uint
	{
		@CXSaveError_None = 0,
		@CXSaveError_Unknown = 1,
		@CXSaveError_TranslationErrors = 2,
		@CXSaveError_InvalidTU = 3,
	}

	public enum CXReparse_Flags : uint
	{
		@CXReparse_None = 0,
	}

	public enum CXTUResourceUsageKind : uint
	{
		@AST = 1,
		@Identifiers = 2,
		@Selectors = 3,
		@GlobalCompletionResults = 4,
		@SourceManagerContentCache = 5,
		@AST_SideTables = 6,
		@SourceManager_Membuffer_Malloc = 7,
		@SourceManager_Membuffer_MMap = 8,
		@ExternalASTSource_Membuffer_Malloc = 9,
		@ExternalASTSource_Membuffer_MMap = 10,
		@Preprocessor = 11,
		@PreprocessingRecord = 12,
		@SourceManager_DataStructures = 13,
		@Preprocessor_HeaderSearch = 14,
		@MEMORY_IN_BYTES_BEGIN = 1,
		@MEMORY_IN_BYTES_END = 14,
		@First = 1,
		@Last = 14,
	}

	public enum CXCursorKind : uint
	{
		@UnexposedDecl = 1,
		@StructDecl = 2,
		@UnionDecl = 3,
		@ClassDecl = 4,
		@EnumDecl = 5,
		@FieldDecl = 6,
		@EnumConstantDecl = 7,
		@FunctionDecl = 8,
		@VarDecl = 9,
		@ParmDecl = 10,
		@ObjCInterfaceDecl = 11,
		@ObjCCategoryDecl = 12,
		@ObjCProtocolDecl = 13,
		@ObjCPropertyDecl = 14,
		@ObjCIvarDecl = 15,
		@ObjCInstanceMethodDecl = 16,
		@ObjCClassMethodDecl = 17,
		@ObjCImplementationDecl = 18,
		@ObjCCategoryImplDecl = 19,
		@TypedefDecl = 20,
		@CXXMethod = 21,
		@Namespace = 22,
		@LinkageSpec = 23,
		@Constructor = 24,
		@Destructor = 25,
		@ConversionFunction = 26,
		@TemplateTypeParameter = 27,
		@NonTypeTemplateParameter = 28,
		@TemplateTemplateParameter = 29,
		@FunctionTemplate = 30,
		@ClassTemplate = 31,
		@ClassTemplatePartialSpecialization = 32,
		@NamespaceAlias = 33,
		@UsingDirective = 34,
		@UsingDeclaration = 35,
		@TypeAliasDecl = 36,
		@ObjCSynthesizeDecl = 37,
		@ObjCDynamicDecl = 38,
		@CXXAccessSpecifier = 39,
		@FirstDecl = 1,
		@LastDecl = 39,
		@FirstRef = 40,
		@ObjCSuperClassRef = 40,
		@ObjCProtocolRef = 41,
		@ObjCClassRef = 42,
		@TypeRef = 43,
		@CXXBaseSpecifier = 44,
		@TemplateRef = 45,
		@NamespaceRef = 46,
		@MemberRef = 47,
		@LabelRef = 48,
		@OverloadedDeclRef = 49,
		@VariableRef = 50,
		@LastRef = 50,
		@FirstInvalid = 70,
		@InvalidFile = 70,
		@NoDeclFound = 71,
		@NotImplemented = 72,
		@InvalidCode = 73,
		@LastInvalid = 73,
		@FirstExpr = 100,
		@UnexposedExpr = 100,
		@DeclRefExpr = 101,
		@MemberRefExpr = 102,
		@CallExpr = 103,
		@ObjCMessageExpr = 104,
		@BlockExpr = 105,
		@IntegerLiteral = 106,
		@FloatingLiteral = 107,
		@ImaginaryLiteral = 108,
		@StringLiteral = 109,
		@CharacterLiteral = 110,
		@ParenExpr = 111,
		@UnaryOperator = 112,
		@ArraySubscriptExpr = 113,
		@BinaryOperator = 114,
		@CompoundAssignOperator = 115,
		@ConditionalOperator = 116,
		@CStyleCastExpr = 117,
		@CompoundLiteralExpr = 118,
		@InitListExpr = 119,
		@AddrLabelExpr = 120,
		@StmtExpr = 121,
		@GenericSelectionExpr = 122,
		@GNUNullExpr = 123,
		@CXXStaticCastExpr = 124,
		@CXXDynamicCastExpr = 125,
		@CXXReinterpretCastExpr = 126,
		@CXXConstCastExpr = 127,
		@CXXFunctionalCastExpr = 128,
		@CXXTypeidExpr = 129,
		@CXXBoolLiteralExpr = 130,
		@CXXNullPtrLiteralExpr = 131,
		@CXXThisExpr = 132,
		@CXXThrowExpr = 133,
		@CXXNewExpr = 134,
		@CXXDeleteExpr = 135,
		@UnaryExpr = 136,
		@ObjCStringLiteral = 137,
		@ObjCEncodeExpr = 138,
		@ObjCSelectorExpr = 139,
		@ObjCProtocolExpr = 140,
		@ObjCBridgedCastExpr = 141,
		@PackExpansionExpr = 142,
		@SizeOfPackExpr = 143,
		@LambdaExpr = 144,
		@ObjCBoolLiteralExpr = 145,
		@ObjCSelfExpr = 146,
		@LastExpr = 146,
		@FirstStmt = 200,
		@UnexposedStmt = 200,
		@LabelStmt = 201,
		@CompoundStmt = 202,
		@CaseStmt = 203,
		@DefaultStmt = 204,
		@IfStmt = 205,
		@SwitchStmt = 206,
		@WhileStmt = 207,
		@DoStmt = 208,
		@ForStmt = 209,
		@GotoStmt = 210,
		@IndirectGotoStmt = 211,
		@ContinueStmt = 212,
		@BreakStmt = 213,
		@ReturnStmt = 214,
		@GCCAsmStmt = 215,
		@AsmStmt = 215,
		@ObjCAtTryStmt = 216,
		@ObjCAtCatchStmt = 217,
		@ObjCAtFinallyStmt = 218,
		@ObjCAtThrowStmt = 219,
		@ObjCAtSynchronizedStmt = 220,
		@ObjCAutoreleasePoolStmt = 221,
		@ObjCForCollectionStmt = 222,
		@CXXCatchStmt = 223,
		@CXXTryStmt = 224,
		@CXXForRangeStmt = 225,
		@SEHTryStmt = 226,
		@SEHExceptStmt = 227,
		@SEHFinallyStmt = 228,
		@MSAsmStmt = 229,
		@NullStmt = 230,
		@DeclStmt = 231,
		@OMPParallelDirective = 232,
		@OMPSimdDirective = 233,
		@OMPForDirective = 234,
		@OMPSectionsDirective = 235,
		@OMPSectionDirective = 236,
		@OMPSingleDirective = 237,
		@OMPParallelForDirective = 238,
		@OMPParallelSectionsDirective = 239,
		@OMPTaskDirective = 240,
		@OMPMasterDirective = 241,
		@OMPCriticalDirective = 242,
		@OMPTaskyieldDirective = 243,
		@OMPBarrierDirective = 244,
		@OMPTaskwaitDirective = 245,
		@OMPFlushDirective = 246,
		@SEHLeaveStmt = 247,
		@OMPOrderedDirective = 248,
		@OMPAtomicDirective = 249,
		@OMPForSimdDirective = 250,
		@OMPParallelForSimdDirective = 251,
		@OMPTargetDirective = 252,
		@OMPTeamsDirective = 253,
		@OMPTaskgroupDirective = 254,
		@LastStmt = 254,
		@TranslationUnit = 300,
		@FirstAttr = 400,
		@UnexposedAttr = 400,
		@IBActionAttr = 401,
		@IBOutletAttr = 402,
		@IBOutletCollectionAttr = 403,
		@CXXFinalAttr = 404,
		@CXXOverrideAttr = 405,
		@AnnotateAttr = 406,
		@AsmLabelAttr = 407,
		@PackedAttr = 408,
		@PureAttr = 409,
		@ConstAttr = 410,
		@NoDuplicateAttr = 411,
		@CUDAConstantAttr = 412,
		@CUDADeviceAttr = 413,
		@CUDAGlobalAttr = 414,
		@CUDAHostAttr = 415,
		@CUDASharedAttr = 416,
		@LastAttr = 416,
		@PreprocessingDirective = 500,
		@MacroDefinition = 501,
		@MacroExpansion = 502,
		@MacroInstantiation = 502,
		@InclusionDirective = 503,
		@FirstPreprocessing = 500,
		@LastPreprocessing = 503,
		@ModuleImportDecl = 600,
		@FirstExtraDecl = 600,
		@LastExtraDecl = 600,
		@OverloadCandidate = 700,
	}

	public enum CXLinkageKind : uint
	{
		@Invalid = 0,
		@NoLinkage = 1,
		@Internal = 2,
		@UniqueExternal = 3,
		@External = 4,
	}

	public enum CXLanguageKind : uint
	{
		@Invalid = 0,
		@C = 1,
		@ObjC = 2,
		@CPlusPlus = 3,
	}

	public enum CXTypeKind : uint
	{
		@Invalid = 0,
		@Unexposed = 1,
		@Void = 2,
		@Bool = 3,
		@Char_U = 4,
		@UChar = 5,
		@Char16 = 6,
		@Char32 = 7,
		@UShort = 8,
		@UInt = 9,
		@ULong = 10,
		@ULongLong = 11,
		@UInt128 = 12,
		@Char_S = 13,
		@SChar = 14,
		@WChar = 15,
		@Short = 16,
		@Int = 17,
		@Long = 18,
		@LongLong = 19,
		@Int128 = 20,
		@Float = 21,
		@Double = 22,
		@LongDouble = 23,
		@NullPtr = 24,
		@Overload = 25,
		@Dependent = 26,
		@ObjCId = 27,
		@ObjCClass = 28,
		@ObjCSel = 29,
		@FirstBuiltin = 2,
		@LastBuiltin = 29,
		@Complex = 100,
		@Pointer = 101,
		@BlockPointer = 102,
		@LValueReference = 103,
		@RValueReference = 104,
		@Record = 105,
		@Enum = 106,
		@Typedef = 107,
		@ObjCInterface = 108,
		@ObjCObjectPointer = 109,
		@FunctionNoProto = 110,
		@FunctionProto = 111,
		@ConstantArray = 112,
		@Vector = 113,
		@IncompleteArray = 114,
		@VariableArray = 115,
		@DependentSizedArray = 116,
		@MemberPointer = 117,
	}

	public enum CXCallingConv : uint
	{
		@Default = 0,
		@C = 1,
		@X86StdCall = 2,
		@X86FastCall = 3,
		@X86ThisCall = 4,
		@X86Pascal = 5,
		@AAPCS = 6,
		@AAPCS_VFP = 7,
		@IntelOclBicc = 9,
		@X86_64Win64 = 10,
		@X86_64SysV = 11,
		@X86VectorCall = 12,
		@Invalid = 100,
		@Unexposed = 200,
	}

	public enum CXTemplateArgumentKind : uint
	{
		@Null = 0,
		@Type = 1,
		@Declaration = 2,
		@NullPtr = 3,
		@Integral = 4,
		@Template = 5,
		@TemplateExpansion = 6,
		@Expression = 7,
		@Pack = 8,
		@Invalid = 9,
	}

	public enum CXTypeLayoutError : int
	{
		@Invalid = -1,
		@Incomplete = -2,
		@Dependent = -3,
		@NotConstantSize = -4,
		@InvalidFieldName = -5,
	}

	public enum CXRefQualifierKind : uint
	{
		@None = 0,
		@LValue = 1,
		@RValue = 2,
	}

	public enum CX_CXXAccessSpecifier : uint
	{
		@InvalidAccessSpecifier = 0,
		@Public = 1,
		@Protected = 2,
		@Private = 3,
	}

	public enum CX_StorageClass : uint
	{
		@Invalid = 0,
		@None = 1,
		@Extern = 2,
		@Static = 3,
		@PrivateExtern = 4,
		@OpenCLWorkGroupLocal = 5,
		@Auto = 6,
		@Register = 7,
	}

	public enum CXChildVisitResult : uint
	{
		@Break = 0,
		@Continue = 1,
		@Recurse = 2,
	}

	public enum CXObjCPropertyAttrKind : uint
	{
		@noattr = 0,
		@readonly = 1,
		@getter = 2,
		@assign = 4,
		@readwrite = 8,
		@retain = 16,
		@copy = 32,
		@nonatomic = 64,
		@setter = 128,
		@atomic = 256,
		@weak = 512,
		@strong = 1024,
		@unsafe_unretained = 2048,
	}

	public enum CXObjCDeclQualifierKind : uint
	{
		@None = 0,
		@In = 1,
		@Inout = 2,
		@Out = 4,
		@Bycopy = 8,
		@Byref = 16,
		@Oneway = 32,
	}

	public enum CXNameRefFlags : uint
	{
		@WantQualifier = 1,
		@WantTemplateArgs = 2,
		@WantSinglePiece = 4,
	}

	public enum CXTokenKind : uint
	{
		@Punctuation = 0,
		@Keyword = 1,
		@Identifier = 2,
		@Literal = 3,
		@Comment = 4,
	}

	public enum CXCompletionChunkKind : uint
	{
		@Optional = 0,
		@TypedText = 1,
		@Text = 2,
		@Placeholder = 3,
		@Informative = 4,
		@CurrentParameter = 5,
		@LeftParen = 6,
		@RightParen = 7,
		@LeftBracket = 8,
		@RightBracket = 9,
		@LeftBrace = 10,
		@RightBrace = 11,
		@LeftAngle = 12,
		@RightAngle = 13,
		@Comma = 14,
		@ResultType = 15,
		@Colon = 16,
		@SemiColon = 17,
		@Equal = 18,
		@HorizontalSpace = 19,
		@VerticalSpace = 20,
	}

	public enum CXCodeComplete_Flags : uint
	{
		@IncludeMacros = 1,
		@IncludeCodePatterns = 2,
		@IncludeBriefComments = 4,
	}

	public enum CXCompletionContext : uint
	{
		@Unexposed = 0,
		@AnyType = 1,
		@AnyValue = 2,
		@ObjCObjectValue = 4,
		@ObjCSelectorValue = 8,
		@CXXClassTypeValue = 16,
		@DotMemberAccess = 32,
		@ArrowMemberAccess = 64,
		@ObjCPropertyAccess = 128,
		@EnumTag = 256,
		@UnionTag = 512,
		@StructTag = 1024,
		@ClassTag = 2048,
		@Namespace = 4096,
		@NestedNameSpecifier = 8192,
		@ObjCInterface = 16384,
		@ObjCProtocol = 32768,
		@ObjCCategory = 65536,
		@ObjCInstanceMessage = 131072,
		@ObjCClassMessage = 262144,
		@ObjCSelectorName = 524288,
		@MacroName = 1048576,
		@NaturalLanguage = 2097152,
		@Unknown = 4194303,
	}

	public enum CXVisitorResult : uint
	{
		@Break = 0,
		@Continue = 1,
	}

	public enum CXResult : uint
	{
		@Success = 0,
		@Invalid = 1,
		@VisitBreak = 2,
	}

	public enum CXIdxEntityKind : uint
	{
		@Unexposed = 0,
		@Typedef = 1,
		@Function = 2,
		@Variable = 3,
		@Field = 4,
		@EnumConstant = 5,
		@ObjCClass = 6,
		@ObjCProtocol = 7,
		@ObjCCategory = 8,
		@ObjCInstanceMethod = 9,
		@ObjCClassMethod = 10,
		@ObjCProperty = 11,
		@ObjCIvar = 12,
		@Enum = 13,
		@Struct = 14,
		@Union = 15,
		@CXXClass = 16,
		@CXXNamespace = 17,
		@CXXNamespaceAlias = 18,
		@CXXStaticVariable = 19,
		@CXXStaticMethod = 20,
		@CXXInstanceMethod = 21,
		@CXXConstructor = 22,
		@CXXDestructor = 23,
		@CXXConversionFunction = 24,
		@CXXTypeAlias = 25,
		@CXXInterface = 26,
	}

	public enum CXIdxEntityLanguage : uint
	{
		@CXIdxEntityLang_None = 0,
		@CXIdxEntityLang_C = 1,
		@CXIdxEntityLang_ObjC = 2,
		@CXIdxEntityLang_CXX = 3,
	}

	public enum CXIdxEntityCXXTemplateKind : uint
	{
		@NonTemplate = 0,
		@Template = 1,
		@TemplatePartialSpecialization = 2,
		@TemplateSpecialization = 3,
	}

	public enum CXIdxAttrKind : uint
	{
		@Unexposed = 0,
		@IBAction = 1,
		@IBOutlet = 2,
		@IBOutletCollection = 3,
	}

	public enum CXIdxDeclInfoFlags : uint
	{
		@Skipped = 1,
	}

	public enum CXIdxObjCContainerKind : uint
	{
		@ForwardRef = 0,
		@Interface = 1,
		@Implementation = 2,
	}

	public enum CXIdxEntityRefKind : uint
	{
		@Direct = 1,
		@Implicit = 2,
	}

	public enum CXIndexOptFlags : uint
	{
		@None = 0,
		@SuppressRedundantRefs = 1,
		@IndexFunctionLocalSymbols = 2,
		@IndexImplicitTemplateInstantiations = 4,
		@SuppressWarnings = 8,
		@SkipParsedBodiesInSession = 16,
	}

	public enum CXCommentKind : uint
	{
		@Null = 0,
		@Text = 1,
		@InlineCommand = 2,
		@HTMLStartTag = 3,
		@HTMLEndTag = 4,
		@Paragraph = 5,
		@BlockCommand = 6,
		@ParamCommand = 7,
		@TParamCommand = 8,
		@VerbatimBlockCommand = 9,
		@VerbatimBlockLine = 10,
		@VerbatimLine = 11,
		@FullComment = 12,
	}

	public enum CXCommentInlineCommandRenderKind : uint
	{
		@Normal = 0,
		@Bold = 1,
		@Monospaced = 2,
		@Emphasized = 3,
	}

	public enum CXCommentParamPassDirection : uint
	{
		@In = 0,
		@Out = 1,
		@InOut = 2,
	}

	[SuppressUnmanagedCodeSecurity]
	public static partial class clang
	{
		private const string libraryPath = "clang37";

		[DllImport(libraryPath, EntryPoint = "clang_getCString", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr getCString(CXString @string);

		[DllImport(libraryPath, EntryPoint = "clang_disposeString", CallingConvention = CallingConvention.Cdecl)]
		public static extern void disposeString(CXString @string);

		[DllImport(libraryPath, EntryPoint = "clang_getBuildSessionTimestamp", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong getBuildSessionTimestamp();

		[DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_create", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXVirtualFileOverlay VirtualFileOverlay_create(uint @options);

		[DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_addFileMapping", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXErrorCode VirtualFileOverlay_addFileMapping(CXVirtualFileOverlay @param0, [MarshalAs(UnmanagedType.LPStr)] string @virtualPath, [MarshalAs(UnmanagedType.LPStr)] string @realPath);

		[DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_setCaseSensitivity", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXErrorCode VirtualFileOverlay_setCaseSensitivity(CXVirtualFileOverlay @param0, int @caseSensitive);

		[DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_writeToBuffer", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXErrorCode VirtualFileOverlay_writeToBuffer(CXVirtualFileOverlay @param0, uint @options, out IntPtr @out_buffer_ptr, out uint @out_buffer_size);

		[DllImport(libraryPath, EntryPoint = "clang_VirtualFileOverlay_dispose", CallingConvention = CallingConvention.Cdecl)]
		public static extern void VirtualFileOverlay_dispose(CXVirtualFileOverlay @param0);

		[DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_create", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXModuleMapDescriptor ModuleMapDescriptor_create(uint @options);

		[DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_setFrameworkModuleName", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXErrorCode ModuleMapDescriptor_setFrameworkModuleName(CXModuleMapDescriptor @param0, [MarshalAs(UnmanagedType.LPStr)] string @name);

		[DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_setUmbrellaHeader", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXErrorCode ModuleMapDescriptor_setUmbrellaHeader(CXModuleMapDescriptor @param0, [MarshalAs(UnmanagedType.LPStr)] string @name);

		[DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_writeToBuffer", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXErrorCode ModuleMapDescriptor_writeToBuffer(CXModuleMapDescriptor @param0, uint @options, out IntPtr @out_buffer_ptr, out uint @out_buffer_size);

		[DllImport(libraryPath, EntryPoint = "clang_ModuleMapDescriptor_dispose", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ModuleMapDescriptor_dispose(CXModuleMapDescriptor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_createIndex", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXIndex createIndex(int @excludeDeclarationsFromPCH, int @displayDiagnostics);

		[DllImport(libraryPath, EntryPoint = "clang_disposeIndex", CallingConvention = CallingConvention.Cdecl)]
		public static extern void disposeIndex(CXIndex @index);

		[DllImport(libraryPath, EntryPoint = "clang_CXIndex_setGlobalOptions", CallingConvention = CallingConvention.Cdecl)]
		public static extern void CXIndex_setGlobalOptions(CXIndex @param0, uint @options);

		[DllImport(libraryPath, EntryPoint = "clang_CXIndex_getGlobalOptions", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint CXIndex_getGlobalOptions(CXIndex @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getFileName", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getFileName(CXFile @SFile);

		[DllImport(libraryPath, EntryPoint = "clang_getFileTime", CallingConvention = CallingConvention.Cdecl)]
		public static extern int getFileTime(CXFile @SFile);

		[DllImport(libraryPath, EntryPoint = "clang_getFileUniqueID", CallingConvention = CallingConvention.Cdecl)]
		public static extern int getFileUniqueID(CXFile @file, out CXFileUniqueID @outID);

		[DllImport(libraryPath, EntryPoint = "clang_isFileMultipleIncludeGuarded", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isFileMultipleIncludeGuarded(CXTranslationUnit @tu, CXFile @file);

		[DllImport(libraryPath, EntryPoint = "clang_getFile", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXFile getFile(CXTranslationUnit @tu, [MarshalAs(UnmanagedType.LPStr)] string @file_name);

		[DllImport(libraryPath, EntryPoint = "clang_File_isEqual", CallingConvention = CallingConvention.Cdecl)]
		public static extern int File_isEqual(CXFile @file1, CXFile @file2);

		[DllImport(libraryPath, EntryPoint = "clang_getNullLocation", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceLocation getNullLocation();

		[DllImport(libraryPath, EntryPoint = "clang_equalLocations", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint equalLocations(CXSourceLocation @loc1, CXSourceLocation @loc2);

		[DllImport(libraryPath, EntryPoint = "clang_getLocation", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceLocation getLocation(CXTranslationUnit @tu, CXFile @file, uint @line, uint @column);

		[DllImport(libraryPath, EntryPoint = "clang_getLocationForOffset", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceLocation getLocationForOffset(CXTranslationUnit @tu, CXFile @file, uint @offset);

		[DllImport(libraryPath, EntryPoint = "clang_Location_isInSystemHeader", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Location_isInSystemHeader(CXSourceLocation @location);

		[DllImport(libraryPath, EntryPoint = "clang_Location_isFromMainFile", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Location_isFromMainFile(CXSourceLocation @location);

		[DllImport(libraryPath, EntryPoint = "clang_getNullRange", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceRange getNullRange();

		[DllImport(libraryPath, EntryPoint = "clang_getRange", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceRange getRange(CXSourceLocation @begin, CXSourceLocation @end);

		[DllImport(libraryPath, EntryPoint = "clang_equalRanges", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint equalRanges(CXSourceRange @range1, CXSourceRange @range2);

		[DllImport(libraryPath, EntryPoint = "clang_Range_isNull", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Range_isNull(CXSourceRange @range);

		[DllImport(libraryPath, EntryPoint = "clang_getExpansionLocation", CallingConvention = CallingConvention.Cdecl)]
		public static extern void getExpansionLocation(CXSourceLocation @location, out CXFile @file, out uint @line, out uint @column, out uint @offset);

		[DllImport(libraryPath, EntryPoint = "clang_getPresumedLocation", CallingConvention = CallingConvention.Cdecl)]
		public static extern void getPresumedLocation(CXSourceLocation @location, out CXString @filename, out uint @line, out uint @column);

		[DllImport(libraryPath, EntryPoint = "clang_getInstantiationLocation", CallingConvention = CallingConvention.Cdecl)]
		public static extern void getInstantiationLocation(CXSourceLocation @location, out CXFile @file, out uint @line, out uint @column, out uint @offset);

		[DllImport(libraryPath, EntryPoint = "clang_getSpellingLocation", CallingConvention = CallingConvention.Cdecl)]
		public static extern void getSpellingLocation(CXSourceLocation @location, out CXFile @file, out uint @line, out uint @column, out uint @offset);

		[DllImport(libraryPath, EntryPoint = "clang_getFileLocation", CallingConvention = CallingConvention.Cdecl)]
		public static extern void getFileLocation(CXSourceLocation @location, out CXFile @file, out uint @line, out uint @column, out uint @offset);

		[DllImport(libraryPath, EntryPoint = "clang_getRangeStart", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceLocation getRangeStart(CXSourceRange @range);

		[DllImport(libraryPath, EntryPoint = "clang_getRangeEnd", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceLocation getRangeEnd(CXSourceRange @range);

		[DllImport(libraryPath, EntryPoint = "clang_getSkippedRanges", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr getSkippedRanges(CXTranslationUnit @tu, CXFile @file);

		[DllImport(libraryPath, EntryPoint = "clang_disposeSourceRangeList", CallingConvention = CallingConvention.Cdecl)]
		public static extern void disposeSourceRangeList(out CXSourceRangeList @ranges);

		[DllImport(libraryPath, EntryPoint = "clang_getNumDiagnosticsInSet", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint getNumDiagnosticsInSet(CXDiagnosticSet @Diags);

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnosticInSet", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXDiagnostic getDiagnosticInSet(CXDiagnosticSet @Diags, uint @Index);

		[DllImport(libraryPath, EntryPoint = "clang_loadDiagnostics", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXDiagnosticSet loadDiagnostics([MarshalAs(UnmanagedType.LPStr)] string @file, out CXLoadDiag_Error @error, out CXString @errorString);

		[DllImport(libraryPath, EntryPoint = "clang_disposeDiagnosticSet", CallingConvention = CallingConvention.Cdecl)]
		public static extern void disposeDiagnosticSet(CXDiagnosticSet @Diags);

		[DllImport(libraryPath, EntryPoint = "clang_getChildDiagnostics", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXDiagnosticSet getChildDiagnostics(CXDiagnostic @D);

		[DllImport(libraryPath, EntryPoint = "clang_getNumDiagnostics", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint getNumDiagnostics(CXTranslationUnit @Unit);

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnostic", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXDiagnostic getDiagnostic(CXTranslationUnit @Unit, uint @Index);

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnosticSetFromTU", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXDiagnosticSet getDiagnosticSetFromTU(CXTranslationUnit @Unit);

		[DllImport(libraryPath, EntryPoint = "clang_disposeDiagnostic", CallingConvention = CallingConvention.Cdecl)]
		public static extern void disposeDiagnostic(CXDiagnostic @Diagnostic);

		[DllImport(libraryPath, EntryPoint = "clang_formatDiagnostic", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString formatDiagnostic(CXDiagnostic @Diagnostic, uint @Options);

		[DllImport(libraryPath, EntryPoint = "clang_defaultDiagnosticDisplayOptions", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint defaultDiagnosticDisplayOptions();

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnosticSeverity", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXDiagnosticSeverity getDiagnosticSeverity(CXDiagnostic @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnosticLocation", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceLocation getDiagnosticLocation(CXDiagnostic @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnosticSpelling", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getDiagnosticSpelling(CXDiagnostic @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnosticOption", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getDiagnosticOption(CXDiagnostic @Diag, out CXString @Disable);

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnosticCategory", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint getDiagnosticCategory(CXDiagnostic @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnosticCategoryName", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getDiagnosticCategoryName(uint @Category);

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnosticCategoryText", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getDiagnosticCategoryText(CXDiagnostic @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnosticNumRanges", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint getDiagnosticNumRanges(CXDiagnostic @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnosticRange", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceRange getDiagnosticRange(CXDiagnostic @Diagnostic, uint @Range);

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnosticNumFixIts", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint getDiagnosticNumFixIts(CXDiagnostic @Diagnostic);

		[DllImport(libraryPath, EntryPoint = "clang_getDiagnosticFixIt", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getDiagnosticFixIt(CXDiagnostic @Diagnostic, uint @FixIt, out CXSourceRange @ReplacementRange);

		[DllImport(libraryPath, EntryPoint = "clang_getTranslationUnitSpelling", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getTranslationUnitSpelling(CXTranslationUnit @CTUnit);

		[DllImport(libraryPath, EntryPoint = "clang_createTranslationUnitFromSourceFile", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXTranslationUnit createTranslationUnitFromSourceFile(CXIndex @CIdx, [MarshalAs(UnmanagedType.LPStr)] string @source_filename, int @num_clang_command_line_args, string[] @clang_command_line_args, uint @num_unsaved_files, CXUnsavedFile[] @unsaved_files);

		[DllImport(libraryPath, EntryPoint = "clang_createTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXTranslationUnit createTranslationUnit(CXIndex @CIdx, [MarshalAs(UnmanagedType.LPStr)] string @ast_filename);

		[DllImport(libraryPath, EntryPoint = "clang_createTranslationUnit2", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXErrorCode createTranslationUnit2(CXIndex @CIdx, [MarshalAs(UnmanagedType.LPStr)] string @ast_filename, out CXTranslationUnit @out_TU);

		[DllImport(libraryPath, EntryPoint = "clang_defaultEditingTranslationUnitOptions", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint defaultEditingTranslationUnitOptions();

		[DllImport(libraryPath, EntryPoint = "clang_parseTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXTranslationUnit parseTranslationUnit(CXIndex @CIdx, [MarshalAs(UnmanagedType.LPStr)] string @source_filename, string[] @command_line_args, int @num_command_line_args, CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, uint @options);

		[DllImport(libraryPath, EntryPoint = "clang_parseTranslationUnit2", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXErrorCode parseTranslationUnit2(CXIndex @CIdx, [MarshalAs(UnmanagedType.LPStr)] string @source_filename, string[] @command_line_args, int @num_command_line_args, CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, uint @options, out CXTranslationUnit @out_TU);

		[DllImport(libraryPath, EntryPoint = "clang_defaultSaveOptions", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint defaultSaveOptions(CXTranslationUnit @TU);

		[DllImport(libraryPath, EntryPoint = "clang_saveTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
		public static extern int saveTranslationUnit(CXTranslationUnit @TU, [MarshalAs(UnmanagedType.LPStr)] string @FileName, uint @options);

		[DllImport(libraryPath, EntryPoint = "clang_disposeTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
		public static extern void disposeTranslationUnit(CXTranslationUnit @param0);

		[DllImport(libraryPath, EntryPoint = "clang_defaultReparseOptions", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint defaultReparseOptions(CXTranslationUnit @TU);

		[DllImport(libraryPath, EntryPoint = "clang_reparseTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
		public static extern int reparseTranslationUnit(CXTranslationUnit @TU, uint @num_unsaved_files, CXUnsavedFile[] @unsaved_files, uint @options);

		[DllImport(libraryPath, EntryPoint = "clang_getTUResourceUsageName", CallingConvention = CallingConvention.Cdecl)]
		public static extern string getTUResourceUsageName(CXTUResourceUsageKind @kind);

		[DllImport(libraryPath, EntryPoint = "clang_getCXTUResourceUsage", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXTUResourceUsage getCXTUResourceUsage(CXTranslationUnit @TU);

		[DllImport(libraryPath, EntryPoint = "clang_disposeCXTUResourceUsage", CallingConvention = CallingConvention.Cdecl)]
		public static extern void disposeCXTUResourceUsage(CXTUResourceUsage @usage);

		[DllImport(libraryPath, EntryPoint = "clang_getNullCursor", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursor getNullCursor();

		[DllImport(libraryPath, EntryPoint = "clang_getTranslationUnitCursor", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursor getTranslationUnitCursor(CXTranslationUnit @param0);

		[DllImport(libraryPath, EntryPoint = "clang_equalCursors", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint equalCursors(CXCursor @param0, CXCursor @param1);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_isNull", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Cursor_isNull(CXCursor @cursor);

		[DllImport(libraryPath, EntryPoint = "clang_hashCursor", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint hashCursor(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorKind", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursorKind getCursorKind(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_isDeclaration", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isDeclaration(CXCursorKind @param0);

		[DllImport(libraryPath, EntryPoint = "clang_isReference", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isReference(CXCursorKind @param0);

		[DllImport(libraryPath, EntryPoint = "clang_isExpression", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isExpression(CXCursorKind @param0);

		[DllImport(libraryPath, EntryPoint = "clang_isStatement", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isStatement(CXCursorKind @param0);

		[DllImport(libraryPath, EntryPoint = "clang_isAttribute", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isAttribute(CXCursorKind @param0);

		[DllImport(libraryPath, EntryPoint = "clang_isInvalid", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isInvalid(CXCursorKind @param0);

		[DllImport(libraryPath, EntryPoint = "clang_isTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isTranslationUnit(CXCursorKind @param0);

		[DllImport(libraryPath, EntryPoint = "clang_isPreprocessing", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isPreprocessing(CXCursorKind @param0);

		[DllImport(libraryPath, EntryPoint = "clang_isUnexposed", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isUnexposed(CXCursorKind @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorLinkage", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXLinkageKind getCursorLinkage(CXCursor @cursor);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorAvailability", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXAvailabilityKind getCursorAvailability(CXCursor @cursor);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorPlatformAvailability", CallingConvention = CallingConvention.Cdecl)]
		public static extern int getCursorPlatformAvailability(CXCursor @cursor, out int @always_deprecated, out CXString @deprecated_message, out int @always_unavailable, out CXString @unavailable_message, out CXPlatformAvailability @availability, int @availability_size);

		[DllImport(libraryPath, EntryPoint = "clang_disposeCXPlatformAvailability", CallingConvention = CallingConvention.Cdecl)]
		public static extern void disposeCXPlatformAvailability(out CXPlatformAvailability @availability);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorLanguage", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXLanguageKind getCursorLanguage(CXCursor @cursor);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXTranslationUnit Cursor_getTranslationUnit(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_createCXCursorSet", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursorSet createCXCursorSet();

		[DllImport(libraryPath, EntryPoint = "clang_disposeCXCursorSet", CallingConvention = CallingConvention.Cdecl)]
		public static extern void disposeCXCursorSet(CXCursorSet @cset);

		[DllImport(libraryPath, EntryPoint = "clang_CXCursorSet_contains", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint CXCursorSet_contains(CXCursorSet @cset, CXCursor @cursor);

		[DllImport(libraryPath, EntryPoint = "clang_CXCursorSet_insert", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint CXCursorSet_insert(CXCursorSet @cset, CXCursor @cursor);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorSemanticParent", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursor getCursorSemanticParent(CXCursor @cursor);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorLexicalParent", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursor getCursorLexicalParent(CXCursor @cursor);

		[DllImport(libraryPath, EntryPoint = "clang_getOverriddenCursors", CallingConvention = CallingConvention.Cdecl)]
		public static extern void getOverriddenCursors(CXCursor @cursor, out IntPtr @overridden, out uint @num_overridden);

		[DllImport(libraryPath, EntryPoint = "clang_disposeOverriddenCursors", CallingConvention = CallingConvention.Cdecl)]
		public static extern void disposeOverriddenCursors(out CXCursor @overridden);

		[DllImport(libraryPath, EntryPoint = "clang_getIncludedFile", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXFile getIncludedFile(CXCursor @cursor);

		[DllImport(libraryPath, EntryPoint = "clang_getCursor", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursor getCursor(CXTranslationUnit @param0, CXSourceLocation @param1);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorLocation", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceLocation getCursorLocation(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorExtent", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceRange getCursorExtent(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType getCursorType(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_getTypeSpelling", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getTypeSpelling(CXType @CT);

		[DllImport(libraryPath, EntryPoint = "clang_getTypedefDeclUnderlyingType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType getTypedefDeclUnderlyingType(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_getEnumDeclIntegerType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType getEnumDeclIntegerType(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_getEnumConstantDeclValue", CallingConvention = CallingConvention.Cdecl)]
		public static extern long getEnumConstantDeclValue(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_getEnumConstantDeclUnsignedValue", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong getEnumConstantDeclUnsignedValue(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_getFieldDeclBitWidth", CallingConvention = CallingConvention.Cdecl)]
		public static extern int getFieldDeclBitWidth(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getNumArguments", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Cursor_getNumArguments(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getArgument", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursor Cursor_getArgument(CXCursor @C, uint @i);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getNumTemplateArguments", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Cursor_getNumTemplateArguments(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentKind", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXTemplateArgumentKind Cursor_getTemplateArgumentKind(CXCursor @C, uint @I);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType Cursor_getTemplateArgumentType(CXCursor @C, uint @I);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentValue", CallingConvention = CallingConvention.Cdecl)]
		public static extern long Cursor_getTemplateArgumentValue(CXCursor @C, uint @I);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getTemplateArgumentUnsignedValue", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong Cursor_getTemplateArgumentUnsignedValue(CXCursor @C, uint @I);

		[DllImport(libraryPath, EntryPoint = "clang_equalTypes", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint equalTypes(CXType @A, CXType @B);

		[DllImport(libraryPath, EntryPoint = "clang_getCanonicalType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType getCanonicalType(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_isConstQualifiedType", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isConstQualifiedType(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_isVolatileQualifiedType", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isVolatileQualifiedType(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_isRestrictQualifiedType", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isRestrictQualifiedType(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_getPointeeType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType getPointeeType(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_getTypeDeclaration", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursor getTypeDeclaration(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_getDeclObjCTypeEncoding", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getDeclObjCTypeEncoding(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_getTypeKindSpelling", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getTypeKindSpelling(CXTypeKind @K);

		[DllImport(libraryPath, EntryPoint = "clang_getFunctionTypeCallingConv", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCallingConv getFunctionTypeCallingConv(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_getResultType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType getResultType(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_getNumArgTypes", CallingConvention = CallingConvention.Cdecl)]
		public static extern int getNumArgTypes(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_getArgType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType getArgType(CXType @T, uint @i);

		[DllImport(libraryPath, EntryPoint = "clang_isFunctionTypeVariadic", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isFunctionTypeVariadic(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorResultType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType getCursorResultType(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_isPODType", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isPODType(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_getElementType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType getElementType(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_getNumElements", CallingConvention = CallingConvention.Cdecl)]
		public static extern long getNumElements(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_getArrayElementType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType getArrayElementType(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_getArraySize", CallingConvention = CallingConvention.Cdecl)]
		public static extern long getArraySize(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_Type_getAlignOf", CallingConvention = CallingConvention.Cdecl)]
		public static extern long Type_getAlignOf(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_Type_getClassType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType Type_getClassType(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_Type_getSizeOf", CallingConvention = CallingConvention.Cdecl)]
		public static extern long Type_getSizeOf(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_Type_getOffsetOf", CallingConvention = CallingConvention.Cdecl)]
		public static extern long Type_getOffsetOf(CXType @T, [MarshalAs(UnmanagedType.LPStr)] string @S);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getOffsetOfField", CallingConvention = CallingConvention.Cdecl)]
		public static extern long Cursor_getOffsetOfField(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_isAnonymous", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint Cursor_isAnonymous(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Type_getNumTemplateArguments", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Type_getNumTemplateArguments(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_Type_getTemplateArgumentAsType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType Type_getTemplateArgumentAsType(CXType @T, uint @i);

		[DllImport(libraryPath, EntryPoint = "clang_Type_getCXXRefQualifier", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXRefQualifierKind Type_getCXXRefQualifier(CXType @T);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_isBitField", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint Cursor_isBitField(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_isVirtualBase", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isVirtualBase(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getCXXAccessSpecifier", CallingConvention = CallingConvention.Cdecl)]
		public static extern CX_CXXAccessSpecifier getCXXAccessSpecifier(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getStorageClass", CallingConvention = CallingConvention.Cdecl)]
		public static extern CX_StorageClass Cursor_getStorageClass(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getNumOverloadedDecls", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint getNumOverloadedDecls(CXCursor @cursor);

		[DllImport(libraryPath, EntryPoint = "clang_getOverloadedDecl", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursor getOverloadedDecl(CXCursor @cursor, uint @index);

		[DllImport(libraryPath, EntryPoint = "clang_getIBOutletCollectionType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType getIBOutletCollectionType(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_visitChildren", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint visitChildren(CXCursor @parent, CXCursorVisitor @visitor, CXClientData @client_data);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorUSR", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getCursorUSR(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCClass", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString constructUSR_ObjCClass([MarshalAs(UnmanagedType.LPStr)] string @class_name);

		[DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCCategory", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString constructUSR_ObjCCategory([MarshalAs(UnmanagedType.LPStr)] string @class_name, [MarshalAs(UnmanagedType.LPStr)] string @category_name);

		[DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCProtocol", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString constructUSR_ObjCProtocol([MarshalAs(UnmanagedType.LPStr)] string @protocol_name);

		[DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCIvar", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString constructUSR_ObjCIvar([MarshalAs(UnmanagedType.LPStr)] string @name, CXString @classUSR);

		[DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCMethod", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString constructUSR_ObjCMethod([MarshalAs(UnmanagedType.LPStr)] string @name, uint @isInstanceMethod, CXString @classUSR);

		[DllImport(libraryPath, EntryPoint = "clang_constructUSR_ObjCProperty", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString constructUSR_ObjCProperty([MarshalAs(UnmanagedType.LPStr)] string @property, CXString @classUSR);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorSpelling", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getCursorSpelling(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getSpellingNameRange", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceRange Cursor_getSpellingNameRange(CXCursor @param0, uint @pieceIndex, uint @options);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorDisplayName", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getCursorDisplayName(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorReferenced", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursor getCursorReferenced(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorDefinition", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursor getCursorDefinition(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_isCursorDefinition", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint isCursorDefinition(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getCanonicalCursor", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursor getCanonicalCursor(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCSelectorIndex", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Cursor_getObjCSelectorIndex(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_isDynamicCall", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Cursor_isDynamicCall(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getReceiverType", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXType Cursor_getReceiverType(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCPropertyAttributes", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint Cursor_getObjCPropertyAttributes(CXCursor @C, uint @reserved);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getObjCDeclQualifiers", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint Cursor_getObjCDeclQualifiers(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_isObjCOptional", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint Cursor_isObjCOptional(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_isVariadic", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint Cursor_isVariadic(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getCommentRange", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceRange Cursor_getCommentRange(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getRawCommentText", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString Cursor_getRawCommentText(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getBriefCommentText", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString Cursor_getBriefCommentText(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getMangling", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString Cursor_getMangling(CXCursor @param0);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getModule", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXModule Cursor_getModule(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_getModuleForFile", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXModule getModuleForFile(CXTranslationUnit @param0, CXFile @param1);

		[DllImport(libraryPath, EntryPoint = "clang_Module_getASTFile", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXFile Module_getASTFile(CXModule @Module);

		[DllImport(libraryPath, EntryPoint = "clang_Module_getParent", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXModule Module_getParent(CXModule @Module);

		[DllImport(libraryPath, EntryPoint = "clang_Module_getName", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString Module_getName(CXModule @Module);

		[DllImport(libraryPath, EntryPoint = "clang_Module_getFullName", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString Module_getFullName(CXModule @Module);

		[DllImport(libraryPath, EntryPoint = "clang_Module_isSystem", CallingConvention = CallingConvention.Cdecl)]
		public static extern int Module_isSystem(CXModule @Module);

		[DllImport(libraryPath, EntryPoint = "clang_Module_getNumTopLevelHeaders", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint Module_getNumTopLevelHeaders(CXTranslationUnit @param0, CXModule @Module);

		[DllImport(libraryPath, EntryPoint = "clang_Module_getTopLevelHeader", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXFile Module_getTopLevelHeader(CXTranslationUnit @param0, CXModule @Module, uint @Index);

		[DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isPureVirtual", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint CXXMethod_isPureVirtual(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isStatic", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint CXXMethod_isStatic(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isVirtual", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint CXXMethod_isVirtual(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_CXXMethod_isConst", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint CXXMethod_isConst(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_getTemplateCursorKind", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursorKind getTemplateCursorKind(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_getSpecializedCursorTemplate", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursor getSpecializedCursorTemplate(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorReferenceNameRange", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceRange getCursorReferenceNameRange(CXCursor @C, uint @NameFlags, uint @PieceIndex);

		[DllImport(libraryPath, EntryPoint = "clang_getTokenKind", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXTokenKind getTokenKind(CXToken @param0);

		[DllImport(libraryPath, EntryPoint = "clang_getTokenSpelling", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getTokenSpelling(CXTranslationUnit @param0, CXToken @param1);

		[DllImport(libraryPath, EntryPoint = "clang_getTokenLocation", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceLocation getTokenLocation(CXTranslationUnit @param0, CXToken @param1);

		[DllImport(libraryPath, EntryPoint = "clang_getTokenExtent", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceRange getTokenExtent(CXTranslationUnit @param0, CXToken @param1);

		[DllImport(libraryPath, EntryPoint = "clang_tokenize", CallingConvention = CallingConvention.Cdecl)]
		public static extern void tokenize(CXTranslationUnit @TU, CXSourceRange @Range, out IntPtr @Tokens, out uint @NumTokens);

		[DllImport(libraryPath, EntryPoint = "clang_annotateTokens", CallingConvention = CallingConvention.Cdecl)]
		public static extern void annotateTokens(CXTranslationUnit @TU, out CXToken @Tokens, uint @NumTokens, out CXCursor @Cursors);

		[DllImport(libraryPath, EntryPoint = "clang_disposeTokens", CallingConvention = CallingConvention.Cdecl)]
		public static extern void disposeTokens(CXTranslationUnit @TU, out CXToken @Tokens, uint @NumTokens);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorKindSpelling", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getCursorKindSpelling(CXCursorKind @Kind);

		[DllImport(libraryPath, EntryPoint = "clang_getDefinitionSpellingAndExtent", CallingConvention = CallingConvention.Cdecl)]
		public static extern void getDefinitionSpellingAndExtent(CXCursor @param0, out IntPtr @startBuf, out IntPtr @endBuf, out uint @startLine, out uint @startColumn, out uint @endLine, out uint @endColumn);

		[DllImport(libraryPath, EntryPoint = "clang_enableStackTraces", CallingConvention = CallingConvention.Cdecl)]
		public static extern void enableStackTraces();

		[DllImport(libraryPath, EntryPoint = "clang_executeOnThread", CallingConvention = CallingConvention.Cdecl)]
		public static extern void executeOnThread(out IntPtr @fn, IntPtr @user_data, uint @stack_size);

		[DllImport(libraryPath, EntryPoint = "clang_getCompletionChunkKind", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCompletionChunkKind getCompletionChunkKind(IntPtr @completion_string, uint @chunk_number);

		[DllImport(libraryPath, EntryPoint = "clang_getCompletionChunkText", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getCompletionChunkText(IntPtr @completion_string, uint @chunk_number);

		[DllImport(libraryPath, EntryPoint = "clang_getCompletionChunkCompletionString", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCompletionString getCompletionChunkCompletionString(IntPtr @completion_string, uint @chunk_number);

		[DllImport(libraryPath, EntryPoint = "clang_getNumCompletionChunks", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint getNumCompletionChunks(IntPtr @completion_string);

		[DllImport(libraryPath, EntryPoint = "clang_getCompletionPriority", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint getCompletionPriority(IntPtr @completion_string);

		[DllImport(libraryPath, EntryPoint = "clang_getCompletionAvailability", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXAvailabilityKind getCompletionAvailability(IntPtr @completion_string);

		[DllImport(libraryPath, EntryPoint = "clang_getCompletionNumAnnotations", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint getCompletionNumAnnotations(IntPtr @completion_string);

		[DllImport(libraryPath, EntryPoint = "clang_getCompletionAnnotation", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getCompletionAnnotation(CXCompletionString @completion_string, uint @annotation_number);

		[DllImport(libraryPath, EntryPoint = "clang_getCompletionParent", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getCompletionParent(CXCompletionString @completion_string, out CXCursorKind @kind);

		[DllImport(libraryPath, EntryPoint = "clang_getCompletionBriefComment", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getCompletionBriefComment(CXCompletionString @completion_string);

		[DllImport(libraryPath, EntryPoint = "clang_getCursorCompletionString", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCompletionString getCursorCompletionString(CXCursor @cursor);

		[DllImport(libraryPath, EntryPoint = "clang_defaultCodeCompleteOptions", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint defaultCodeCompleteOptions();

		[DllImport(libraryPath, EntryPoint = "clang_codeCompleteAt", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr codeCompleteAt(CXTranslationUnit @TU, [MarshalAs(UnmanagedType.LPStr)] string @complete_filename, uint @complete_line, uint @complete_column, CXUnsavedFile[] @unsaved_files, uint @num_unsaved_files, uint @options);

		[DllImport(libraryPath, EntryPoint = "clang_sortCodeCompletionResults", CallingConvention = CallingConvention.Cdecl)]
		public static extern void sortCodeCompletionResults(out CXCompletionResult @Results, uint @NumResults);

		[DllImport(libraryPath, EntryPoint = "clang_disposeCodeCompleteResults", CallingConvention = CallingConvention.Cdecl)]
		public static extern void disposeCodeCompleteResults(IntPtr @Results);

		[DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetNumDiagnostics", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint codeCompleteGetNumDiagnostics(out CXCodeCompleteResults @Results);

		[DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetDiagnostic", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXDiagnostic codeCompleteGetDiagnostic(out CXCodeCompleteResults @Results, uint @Index);

		[DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetContexts", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong codeCompleteGetContexts(out CXCodeCompleteResults @Results);

		[DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetContainerKind", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCursorKind codeCompleteGetContainerKind(out CXCodeCompleteResults @Results, out uint @IsIncomplete);

		[DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetContainerUSR", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString codeCompleteGetContainerUSR(out CXCodeCompleteResults @Results);

		[DllImport(libraryPath, EntryPoint = "clang_codeCompleteGetObjCSelector", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString codeCompleteGetObjCSelector(out CXCodeCompleteResults @Results);

		[DllImport(libraryPath, EntryPoint = "clang_getClangVersion", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString getClangVersion();

		[DllImport(libraryPath, EntryPoint = "clang_toggleCrashRecovery", CallingConvention = CallingConvention.Cdecl)]
		public static extern void toggleCrashRecovery(uint @isEnabled);

		[DllImport(libraryPath, EntryPoint = "clang_getInclusions", CallingConvention = CallingConvention.Cdecl)]
		public static extern void getInclusions(CXTranslationUnit @tu, CXInclusionVisitor @visitor, CXClientData @client_data);

		[DllImport(libraryPath, EntryPoint = "clang_getRemappings", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXRemapping getRemappings([MarshalAs(UnmanagedType.LPStr)] string @path);

		[DllImport(libraryPath, EntryPoint = "clang_getRemappingsFromFileList", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXRemapping getRemappingsFromFileList(out IntPtr @filePaths, uint @numFiles);

		[DllImport(libraryPath, EntryPoint = "clang_remap_getNumFiles", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint remap_getNumFiles(CXRemapping @param0);

		[DllImport(libraryPath, EntryPoint = "clang_remap_getFilenames", CallingConvention = CallingConvention.Cdecl)]
		public static extern void remap_getFilenames(CXRemapping @param0, uint @index, out CXString @original, out CXString @transformed);

		[DllImport(libraryPath, EntryPoint = "clang_remap_dispose", CallingConvention = CallingConvention.Cdecl)]
		public static extern void remap_dispose(CXRemapping @param0);

		[DllImport(libraryPath, EntryPoint = "clang_findReferencesInFile", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXResult findReferencesInFile(CXCursor @cursor, CXFile @file, CXCursorAndRangeVisitor @visitor);

		[DllImport(libraryPath, EntryPoint = "clang_findIncludesInFile", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXResult findIncludesInFile(CXTranslationUnit @TU, CXFile @file, CXCursorAndRangeVisitor @visitor);

		[DllImport(libraryPath, EntryPoint = "clang_index_isEntityObjCContainerKind", CallingConvention = CallingConvention.Cdecl)]
		public static extern int index_isEntityObjCContainerKind(CXIdxEntityKind @param0);

		[DllImport(libraryPath, EntryPoint = "clang_index_getObjCContainerDeclInfo", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr index_getObjCContainerDeclInfo(out CXIdxDeclInfo @param0);

		[DllImport(libraryPath, EntryPoint = "clang_index_getObjCInterfaceDeclInfo", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr index_getObjCInterfaceDeclInfo(out CXIdxDeclInfo @param0);

		[DllImport(libraryPath, EntryPoint = "clang_index_getObjCCategoryDeclInfo", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr index_getObjCCategoryDeclInfo(out CXIdxDeclInfo @param0);

		[DllImport(libraryPath, EntryPoint = "clang_index_getObjCProtocolRefListInfo", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr index_getObjCProtocolRefListInfo(out CXIdxDeclInfo @param0);

		[DllImport(libraryPath, EntryPoint = "clang_index_getObjCPropertyDeclInfo", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr index_getObjCPropertyDeclInfo(out CXIdxDeclInfo @param0);

		[DllImport(libraryPath, EntryPoint = "clang_index_getIBOutletCollectionAttrInfo", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr index_getIBOutletCollectionAttrInfo(out CXIdxAttrInfo @param0);

		[DllImport(libraryPath, EntryPoint = "clang_index_getCXXClassDeclInfo", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr index_getCXXClassDeclInfo(out CXIdxDeclInfo @param0);

		[DllImport(libraryPath, EntryPoint = "clang_index_getClientContainer", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXIdxClientContainer index_getClientContainer(out CXIdxContainerInfo @param0);

		[DllImport(libraryPath, EntryPoint = "clang_index_setClientContainer", CallingConvention = CallingConvention.Cdecl)]
		public static extern void index_setClientContainer(out CXIdxContainerInfo @param0, CXIdxClientContainer @param1);

		[DllImport(libraryPath, EntryPoint = "clang_index_getClientEntity", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXIdxClientEntity index_getClientEntity(out CXIdxEntityInfo @param0);

		[DllImport(libraryPath, EntryPoint = "clang_index_setClientEntity", CallingConvention = CallingConvention.Cdecl)]
		public static extern void index_setClientEntity(out CXIdxEntityInfo @param0, CXIdxClientEntity @param1);

		[DllImport(libraryPath, EntryPoint = "clang_IndexAction_create", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXIndexAction IndexAction_create(CXIndex @CIdx);

		[DllImport(libraryPath, EntryPoint = "clang_IndexAction_dispose", CallingConvention = CallingConvention.Cdecl)]
		public static extern void IndexAction_dispose(CXIndexAction @param0);

		[DllImport(libraryPath, EntryPoint = "clang_indexSourceFile", CallingConvention = CallingConvention.Cdecl)]
		public static extern int indexSourceFile(CXIndexAction @param0, CXClientData @client_data, out IndexerCallbacks @index_callbacks, uint @index_callbacks_size, uint @index_options, [MarshalAs(UnmanagedType.LPStr)] string @source_filename, string[] @command_line_args, int @num_command_line_args, out CXUnsavedFile @unsaved_files, uint @num_unsaved_files, out CXTranslationUnit @out_TU, uint @TU_options);

		[DllImport(libraryPath, EntryPoint = "clang_indexTranslationUnit", CallingConvention = CallingConvention.Cdecl)]
		public static extern int indexTranslationUnit(CXIndexAction @param0, CXClientData @client_data, out IndexerCallbacks @index_callbacks, uint @index_callbacks_size, uint @index_options, CXTranslationUnit @param5);

		[DllImport(libraryPath, EntryPoint = "clang_indexLoc_getFileLocation", CallingConvention = CallingConvention.Cdecl)]
		public static extern void indexLoc_getFileLocation(CXIdxLoc @loc, out CXIdxClientFile @indexFile, out CXFile @file, out uint @line, out uint @column, out uint @offset);

		[DllImport(libraryPath, EntryPoint = "clang_indexLoc_getCXSourceLocation", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXSourceLocation indexLoc_getCXSourceLocation(CXIdxLoc @loc);

		[DllImport(libraryPath, EntryPoint = "clang_Type_visitFields", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint Type_visitFields(CXType @T, CXFieldVisitor @visitor, CXClientData @client_data);

		[DllImport(libraryPath, EntryPoint = "clang_Cursor_getParsedComment", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXComment Cursor_getParsedComment(CXCursor @C);

		[DllImport(libraryPath, EntryPoint = "clang_Comment_getKind", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCommentKind Comment_getKind(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_Comment_getNumChildren", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint Comment_getNumChildren(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_Comment_getChild", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXComment Comment_getChild(CXComment @Comment, uint @ChildIdx);

		[DllImport(libraryPath, EntryPoint = "clang_Comment_isWhitespace", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint Comment_isWhitespace(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_InlineContentComment_hasTrailingNewline", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint InlineContentComment_hasTrailingNewline(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_TextComment_getText", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString TextComment_getText(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getCommandName", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString InlineCommandComment_getCommandName(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getRenderKind", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCommentInlineCommandRenderKind InlineCommandComment_getRenderKind(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getNumArgs", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint InlineCommandComment_getNumArgs(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_InlineCommandComment_getArgText", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString InlineCommandComment_getArgText(CXComment @Comment, uint @ArgIdx);

		[DllImport(libraryPath, EntryPoint = "clang_HTMLTagComment_getTagName", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString HTMLTagComment_getTagName(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_HTMLStartTagComment_isSelfClosing", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint HTMLStartTagComment_isSelfClosing(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_HTMLStartTag_getNumAttrs", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint HTMLStartTag_getNumAttrs(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_HTMLStartTag_getAttrName", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString HTMLStartTag_getAttrName(CXComment @Comment, uint @AttrIdx);

		[DllImport(libraryPath, EntryPoint = "clang_HTMLStartTag_getAttrValue", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString HTMLStartTag_getAttrValue(CXComment @Comment, uint @AttrIdx);

		[DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getCommandName", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString BlockCommandComment_getCommandName(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getNumArgs", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint BlockCommandComment_getNumArgs(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getArgText", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString BlockCommandComment_getArgText(CXComment @Comment, uint @ArgIdx);

		[DllImport(libraryPath, EntryPoint = "clang_BlockCommandComment_getParagraph", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXComment BlockCommandComment_getParagraph(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_getParamName", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString ParamCommandComment_getParamName(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_isParamIndexValid", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ParamCommandComment_isParamIndexValid(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_getParamIndex", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ParamCommandComment_getParamIndex(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_isDirectionExplicit", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ParamCommandComment_isDirectionExplicit(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_ParamCommandComment_getDirection", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXCommentParamPassDirection ParamCommandComment_getDirection(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_getParamName", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString TParamCommandComment_getParamName(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_isParamPositionValid", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint TParamCommandComment_isParamPositionValid(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_getDepth", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint TParamCommandComment_getDepth(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_TParamCommandComment_getIndex", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint TParamCommandComment_getIndex(CXComment @Comment, uint @Depth);

		[DllImport(libraryPath, EntryPoint = "clang_VerbatimBlockLineComment_getText", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString VerbatimBlockLineComment_getText(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_VerbatimLineComment_getText", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString VerbatimLineComment_getText(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_HTMLTagComment_getAsString", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString HTMLTagComment_getAsString(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_FullComment_getAsHTML", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString FullComment_getAsHTML(CXComment @Comment);

		[DllImport(libraryPath, EntryPoint = "clang_FullComment_getAsXML", CallingConvention = CallingConvention.Cdecl)]
		public static extern CXString FullComment_getAsXML(CXComment @Comment);

	}
}
