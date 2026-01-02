---
layout: post
title: "Testing your data repositories"
subtitle: "Avoiding dependency on a data layer."
author: "Devon Burriss"
comments: true
permalink: testing-your-data-repositories
published: true
topics: [engineering-practices, platforms-runtime]
keywords: [Software Development, Testing, Programming, SOLID, OOP, Java, TDD]
---

---
> Avoiding dependency on a data layer.

My solution was to use an in-memory H2 database (http://www.h2database.com/html/main.html) which can be created and dropped on a per test basis. To do this I used the Command Pattern (http://en.wikipedia.org/wiki/Command_pattern) to create and then drop the table for each test. In case you are not familiar with the command pattern: 

![library](/img/posts/2014/books-800-medium.jpg)

## Command Pattern 

The command pattern is pretty simple. You define an interface with the method that will be called to execute some functionality.

{% highlight java %}
public interface Command {
	void execute() throws Exception;
}
{% endhighlight %}

## The Solution 

So this is what the end result looks like. How you execute you commands is up to you but in case you are looking for the details I have included them further down in the article.

{% highlight java %}
public class CommitteeTableCommandTest {         
	private String connectionString = "jdbc:h2:~/test";     
	
	@Test    
	public void create_NewCommitteeRecord_PersistsToDb() throws Exception {                 
		try(Database database = new H2DatabaseImpl(connectionString, "", "")){
				Command cc = new CreateCommitteeTableCommand(database);          
				cc.execute();                         
				CommitteeEntity entity = new CommitteeEntity();            
				entity.setName("Test");    
				entity.setMandate("Blah Blah");
                
				CommitteeRepository sut = new CommitteeRepositoryImpl(database);
				sut.create(entity);
				Assert.assertNotNull(sut.getByName("Test").get(0));
				Command cd = new DropCommitteeTableCommand(database);
				cd.execute();
		}         
	
		Assert.assertTrue(true);    	
	} 
}
{% endhighlight %}

### The Details 

For the creation and dropping of the table I created a generic abstract base class for each. I am using OrmLite (http://ormlite.com/) (the Java library, not C# one – which is unrelated) for my Object Relational Mapper. This gives me a database agnostic way for handling the mundane database tasks without mixing my Java and SQL. You could quite easily write SQL for this, as long as you take any differences in database providers into consideration. On to the solution… 

*Base create command*
{% highlight java %}
public abstract class BaseCreateTableCommand<T> implements Command { 
	
	private Database database;    
	private Class<T> typeOfT;
	
	@SuppressWarnings("unchecked")    
	public BaseCreateTableCommand(Database database){        
		this.database = database;
		ParameterizedType genericSuperclass = (ParameterizedType) getClass().getGenericSuperclass();
	    Type type = genericSuperclass.getActualTypeArguments()[0];
	    if (type instanceof Class) {
	      this.typeOfT = (Class<T>) type;
	    } else if (type instanceof ParameterizedType) {
	      this.typeOfT = (Class<T>) ((ParameterizedType)type).getRawType();
	    }
	}

	protected void createTableIfNotExists() throws Exception {        
		ConnectionSource connectionSource = new JdbcConnectionSource(database.getConnectionUri(), database.getUsername(), database.getPassword());  
		TableUtils.createTableIfNotExists(connectionSource, typeOfT);        
		connectionSource.close();    
	}  

	public void execute() throws Exception {                 
		this.createTableIfNotExists();    
	} 
}
{% endhighlight %}

*Base drop command*
{% highlight java %}
public abstract class BaseDropTableCommand<T> implements Command {     
	private Database database;    
	private Class<T> typeOfT;         
	
	@SuppressWarnings("unchecked")    
	public BaseDropTableCommand(Database database){        
		this.database = database;        
		this.typeOfT = (Class<T>)((ParameterizedType)getClass().getGenericSuperclass()).getActualTypeArguments()[0];
	}

	protected void dropTable(Boolean ignoreErrors) throws Exception {

		ConnectionSource connectionSource = new JdbcConnectionSource(database.getConnectionUri(), database.getUsername(), database.getPassword()); 
		TableUtils.dropTable(connectionSource, typeOfT, ignoreErrors);
		connectionSource.close();
	}     
	
	@Override    
	public void execute() throws Exception {
		this.dropTable(true);    
	} 
}
{% endhighlight %}

Next, we inherit from these two classes to flesh out the create and drop commands. 
*Create command implementation*
{% highlight java %}
public class CreateCommitteeTableCommand extends BaseCreateTableCommand<CommitteeEntity> {     
	public CreateCommitteeTableCommand(Database database) {
		super(database);    
	}
}
{% endhighlight %}

*Drop command implementation*
{% highlight java %}
public class DropCommitteeTableCommand extends BaseDropTableCommand<CommitteeEntity> {     
	public DropCommitteeTableCommand(Database database){
		super(database);
	} 
}
{% endhighlight %}

The only other piece is the Database abstraction, which I have my doubts about so I would
not recommend copying :)

*Database abstraction*
{% highlight java %}
public abstract class Database implements AutoCloseable {

	private static final int MAX_CONNECTIONS_PER_PARTITION = 2;

	private static final int MIN_CONNECTIONS_PER_PARTITION = 1;

	private static final int LOGIN_TIMEOUT = 10;

	protected final Logger logger = LoggerFactory.getLogger(getClass());
	
	protected String connectionUri;
	protected String username;
	protected String password;
	
	protected BoneCP connectionPool = null;

	public Database() {
		super();
	}

	public Connection getConnection() throws SQLException {
		logger.trace("getConnection called.");
		return getPooledConnection();
	}
	
	public String getConnectionUri(){
		return this.connectionUri;
	}
	
	public String getUsername(){
		return this.username;
	}
	
	public String getPassword(){
		return this.password;
	}

	public abstract String getDriver();

	
	public void close() throws Exception {
		logger.trace("close called (this is close() on the database...not a single connection).");
		if(this.connectionPool != null)
			this.connectionPool.shutdown();
		
		this.connectionPool = null;
	}

	protected void setup(String driver, String connectionUri, String username, String password) throws ClassNotFoundException, SQLException {
		logger.trace("setup called.");
		try {
			Class.forName(driver);

			this.connectionUri = connectionUri;
			this.username = username;
			this.password = password;
			DriverManager.setLoginTimeout(LOGIN_TIMEOUT);

		} catch (ClassNotFoundException e) {
			logger.error(e.getMessage(), e);
			throw e;
		}
	}

	private Connection getPooledConnection() throws SQLException {
		Connection conn;

		if(connectionPool == null)
			setupConnectionPool(connectionUri, username, password);
		
		conn = connectionPool.getConnection();
		return conn;
	}

	private void setupConnectionPool(String connectionUri, String username,	String password) throws SQLException {
		
		BoneCPConfig config = new BoneCPConfig();
		config.setJdbcUrl(connectionUri);
		config.setUsername(username); 
		config.setPassword(password);
		config.setMinConnectionsPerPartition(MIN_CONNECTIONS_PER_PARTITION);
		config.setMaxConnectionsPerPartition(MAX_CONNECTIONS_PER_PARTITION);
		config.setPartitionCount(1);
		config.setLazyInit(true);
		connectionPool = new BoneCP(config);
	}
}
{% endhighlight %}

*H2 implementation*
{% highlight java %}
public class H2DatabaseImpl extends Database {

	private final String driver = "org.h2.Driver";
	
	public H2DatabaseImpl(String connectionUri, String username, String password) throws ClassNotFoundException, SQLException{
		super();
		this.setup(driver, connectionUri, username, password);
	}
	
	@Override
	public String getDriver() {
		return driver;
	}
}
{% endhighlight %}

*Just for kicks...*

I created a command queue, which itself is a command to enumerate through and execute a list of commands. Here just because its useful, not for purposes of this example. You can chain your inserts and then your drops into two commands using this.

{% highlight java %}
public class CommandQueue implements Command {     
	private List<Command> commands;    
	private Boolean breakOnError = true;         
	
	public CommandQueue(List<Command> commands, Boolean breakOnError){        
		if(commands == null)            
			throw new IllegalArgumentException("commands");        
		
		this.commands = commands;                 
		
		if(breakOnError != null)            
			this.breakOnError = breakOnError;    
	}    
	
	@Override    
	public void execute() throws Exception {        
		int pos = 0;        
		try {            
			pos = executeImpl(pos);
		} catch (Exception e) {            
			if(this.breakOnError)                
				throw e;        
		}    
	}    
	
	private int executeImpl(int pos) throws Exception {        
		int size = this.commands.size();        
		
		for (int i = pos; i < size; i++) {            
			try {                
				this.commands.get(pos).execute();                
				pos++;            
			} catch (Exception e) {                
				if(this.breakOnError)                    
					throw e;                
				executeImpl(++pos);            
			}        
		}        
		
		return pos;    
	} 
}
{% endhighlight %}

Let me know if you found this useful, or if you have a better way for testing your data persistence...



