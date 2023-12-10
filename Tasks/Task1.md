# Session 1: Introduction to SemanticKernel

## Prerequisite
Before starting this session, ensure you have received the Azure OpenAI API keys via email.

## Task Steps


TODO: Ruslan please fix next step
### Step 1: Project Setup
- **1.1 Create a new project directory**:
  - Use the command line or terminal to create a new folder.
- **1.2 Initialize a Node.js project**:
  - Run `npm init` to create a `package.json` file.


TODO: Ruslan please fix next step
### Step 2: Install NPM Packages
- **2.1 Install LangChain and other dependencies**:
  - Run `npm install langchain axios dotenv`.


TODO: Может добавим файл для конфигурирования и примеры конфигов?
### Step 3: Azure OpenAI API Integration
- **3.1 Configure .env file**:
  - Create a `.env` file in your project root.
  - Add your Azure OpenAI API keys and endpoint to the `.env` file for secure access.
- **3.2 Create your first call to Azure OpenAI LLM model**:
  -  Develop JavaScript code to make the initial request to the Azure OpenAI LLM model using SemanticKernel.
  -  Replace LLM Model With Chat Model, and check that it works as expected.
  -  With the same PromptTemplate, change the Temperature to see how the output changes.

<details>
  <summary>Hint</summary>

Provide all four parameters for Azure: API_KEY, API_VERSION, BASE_PATH, and DEPLOYMENT_NAME

</details>

### Step 4: Register on HuggingFace and Get an API Key

- **4.1 Register or Login:**
  - Visit the Hugging Face website: [https://huggingface.co/](https://huggingface.co/)

- **4.2 Generate an API Token:**
  - Log in to Hugging Face.
  - Click on your username and select "Settings".
  - In the "Access Tokens" tab, click the "New Token" button.
  - Enter a descriptive name for your token.
  - Select the appropriate permissions for your token.
  - Click the "Generate token" button.
  - Copy the generated API token and save it securely.

### Step 5: HuggingFace Model Call
- **5.1 Integrate HuggingFace model**:
  - Replace the Azure OpenAI model with a HuggingFace model.
  - Check that code with new model works as expected.

<details>
  <summary>Hint</summary>

You need replace AddAzureOpenAIChatCompletion with AddHuggingFaceChatCompletion
Depending on the nuget package version, you may need to stop worrning with pragma: #pragma warning disable SKEXP0020

</details>


### Final Steps
- **Lets Play with different models**: [https://huggingface.co/models](https://huggingface.co/models) 
